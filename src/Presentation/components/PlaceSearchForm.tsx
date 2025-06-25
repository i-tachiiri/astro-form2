'use client';
import { useState, FormEvent } from 'react';

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

  async function submit(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();
    const q = query.trim();
    if (!q) {
      setSuggestions([]);
      return;
    }
    const resp = await fetch(`${baseUrl}/api/map?query=${encodeURIComponent(q)}`);
    if (resp.ok) {
      const data = await resp.json();
      const results = data.results ?? data.Results ?? [];
      if (Array.isArray(results)) {
        const normalized = results.map((r: any) => ({
          place_id: r.place_id ?? r.placeId ?? r.PlaceId,
          name: r.name ?? r.Name,
          description: r.description ?? r.Description,
        }));
        setSuggestions(normalized);
      } else {
        setSuggestions([]);
      }
    }
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
  }

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
    <form onSubmit={submit}>
      <input value={query} onChange={(e) => setQuery(e.target.value)} placeholder="Search place" />
      {suggestions.length > 0 && (
        <ul className="suggestions">
          {suggestions.map((s) => (
            <li key={s.place_id} onClick={() => select(s)}>{s.description}</li>
          ))}
        </ul>
      )}
    </form>
  );
}
