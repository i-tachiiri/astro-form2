'use client';
import { useState, useEffect } from 'react';
import PlaceSearchForm from '../../components/PlaceSearchForm';
import PlaceSearchResult from '../../components/PlaceSearchResult';

interface AccessLog {
  id: string;
  sessionId: string;
  accessedAt: string;
}

interface PlaceDetails {
  place_id: string;
  name: string;
  address: string;
  lat: number;
  lng: number;
  map_url: string;
}

export default function SearchPage() {
  const [result, setResult] = useState<PlaceDetails | undefined>();

  useEffect(() => {
    const log: AccessLog = {
      id: crypto.randomUUID(),
      sessionId: crypto.randomUUID(),
      accessedAt: new Date().toISOString(),
    };
    fetch('/api/log/access', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(log) });
    fetch('/api/test-data/initialize', { method: 'POST' }).catch(() => {});
    fetch('/api/test-data/seed', { method: 'POST' }).catch(() => {});
  }, []);

  return (
    <div>
      <PlaceSearchForm onSelected={setResult} />
      <PlaceSearchResult result={result} />
    </div>
  );
}
