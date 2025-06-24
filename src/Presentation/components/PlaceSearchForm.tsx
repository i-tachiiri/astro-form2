'use client';
import { useState, ChangeEvent } from 'react';

interface SearchResultItem {
  place_id: string;
  name: string;
  description: string;
}

interface Props {
  onSelected: (detail: PlaceDetails) => void;
}

interface PlaceDetails {
  place_id: string;
  name: string;
  address: string;
  lat: number;
  lng: number;
  map_url: string;
}

export default function PlaceSearchForm({ onSelected }: Props) {
  const [query, setQuery] = useState('');
  const [suggestions, setSuggestions] = useState<SearchResultItem[]>([]);

  async function loadSuggestions(e: ChangeEvent<HTMLInputElement>) {
    const q = e.target.value;
    setQuery(q);
    if (!q) {
      setSuggestions([]);
      return;
    }
    const resp = await fetch(`/api/map?query=${encodeURIComponent(q)}`);
    if (resp.ok) {
      const data = await resp.json();
      const results = data.results ?? data.Results ?? [];
      if (Array.isArray(results)) {
        const normalized = results.map((r: any) => ({
          place_id: r.place_id ?? r.placeId,
          name: r.name,
          description: r.description,
        }));
        setSuggestions(normalized);
      } else {
        setSuggestions([]);
      }
    }
  }

  async function select(item: SearchResultItem) {
    setQuery(item.description);
    setSuggestions([]);
    const resp = await fetch(`/api/map/${item.place_id}`);
    if (resp.ok) {
      const detail = await resp.json();
      const normalized: PlaceDetails = {
        place_id: detail.place_id ?? detail.placeId,
        name: detail.name,
        address: detail.address,
        lat: detail.lat,
        lng: detail.lng,
        map_url: detail.map_url ?? detail.mapUrl,
      };
      onSelected(normalized);
    }
  }

  return (
    <div>
      <input value={query} onChange={loadSuggestions} placeholder="Search place" />
      {suggestions.length > 0 && (
        <ul className="suggestions">
          {suggestions.map((s) => (
            <li key={s.place_id} onClick={() => select(s)}>{s.description}</li>
          ))}
        </ul>
      )}
    </div>
  );
}
