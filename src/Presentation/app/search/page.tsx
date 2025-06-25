'use client';
import { useState, useEffect, useRef } from 'react';
import PlaceSearchForm from '../../components/PlaceSearchForm';
import PlaceSearchResult from '../../components/PlaceSearchResult';

interface AccessLog {
  id: string;
  session_id: string;
  accessed_at: string;
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

export default function SearchPage() {
  const [result, setResult] = useState<PlaceDetails | undefined>();
  const sessionIdRef = useRef<string>(crypto.randomUUID());

  useEffect(() => {
    const log: AccessLog = {
      id: crypto.randomUUID(),
      session_id: sessionIdRef.current,
      accessed_at: new Date().toISOString(),
    };
    fetch('/api/log/access', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(log) });
    fetch('/api/test-data/initialize', { method: 'POST' }).catch(() => {});
    fetch('/api/test-data/seed', { method: 'POST' }).catch(() => {});
  }, []);

  return (
    <div>
      <PlaceSearchForm onSelected={setResult} sessionId={sessionIdRef.current} />
      <PlaceSearchResult result={result} />
    </div>
  );
}
