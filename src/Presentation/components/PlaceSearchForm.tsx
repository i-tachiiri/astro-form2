'use client';
import { useState, useRef } from 'react';
import { Autocomplete, AutocompleteItem } from '@heroui/autocomplete';

interface SearchResultItem {
  place_id: string;
  name: string;
  description: string;
}

interface Props {
  onSelected: (detail: PlaceDetails) => void;
  sessionId: string;
}

interface PlaceDetails {
  place_id: string;
  name: string;
  address: string;
  lat: number;
  lng: number;
  map_url: string;
}

const baseUrl = process.env.NEXT_PUBLIC_API_BASE_URL ?? '';

export default function PlaceSearchForm({ onSelected, sessionId }: Props) {
  const [query, setQuery] = useState('');
  const [suggestions, setSuggestions] = useState<SearchResultItem[]>([]);
  const timerRef = useRef<NodeJS.Timeout>();

  const handleChange = (value: string) => {
    setQuery(value);
    if (timerRef.current) {
      clearTimeout(timerRef.current);
    }
    const q = value.trim();
    if (!q) {
      setSuggestions([]);
      return;
    }
    const currentScroll = window.scrollY;
    timerRef.current = setTimeout(async () => {
      const resp = await fetch(`${baseUrl}/api/map?query=${encodeURIComponent(q)}`);
      if (resp.ok) {
        const data = await resp.json();
        const results = data.results ?? data.Results ?? [];
        if (Array.isArray(results)) {
          const normalized = results.slice(0, 5).map((r: any) => ({
            place_id: r.place_id ?? r.placeId ?? r.PlaceId,
            name: r.name ?? r.Name,
            description: r.description ?? r.Description,
          }));
          setSuggestions(normalized);
        } else {
          setSuggestions([]);
        }
      }
      window.scrollTo({ top: currentScroll });
      const actionLog = {
        id: crypto.randomUUID(),
        session_id: sessionId,
        action_name: 'search',
        actioned_at: new Date().toISOString(),
      };
      fetch(`${baseUrl}/api/log/action`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(actionLog),
      }).catch(() => {});
    }, 300);
  };

  async function select(item: SearchResultItem) {
    setQuery(item.description);
    setSuggestions([]);
    const resp = await fetch(`${baseUrl}/api/map/${item.place_id}`);
    if (resp.ok) {
      const detail = await resp.json();
      const normalized: PlaceDetails = {
        place_id: detail.place_id ?? detail.placeId ?? detail.PlaceId,
        name: detail.name ?? detail.Name,
        address: detail.address ?? detail.Address,
        lat: detail.lat ?? detail.Lat,
        lng: detail.lng ?? detail.Lng,
        map_url: detail.map_url ?? detail.mapUrl ?? detail.MapUrl,
      };
      onSelected(normalized);
      const searchLog = {
        id: crypto.randomUUID(),
        session_id: sessionId,
        place_id: normalized.place_id,
        query,
        lat: normalized.lat,
        lng: normalized.lng,
        searched_at: new Date().toISOString(),
      };
      fetch(`${baseUrl}/api/log/search_result`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(searchLog),
      }).catch(() => {});
    }
  }

  return (
    <Autocomplete
      aria-label="Search place"
      className="w-full"
      popoverProps={{ shouldBlockScroll: true }}
      listboxProps={{ emptyContent: null, itemClasses: { base: 'cursor-pointer' } }}
      inputValue={query}
      items={suggestions}
      onInputChange={handleChange}
      onSelectionChange={(key) => {
        const item = suggestions.find((s) => s.place_id === key);
        if (item) select(item);
      }}
    >
      {(item) => (
        <AutocompleteItem key={item.place_id}>{item.description}</AutocompleteItem>
      )}
    </Autocomplete>
  );
}
