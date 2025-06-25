'use client';
import { useEffect, useRef } from 'react';

const baseUrl = process.env.NEXT_PUBLIC_API_BASE_URL ?? '';

export default function AccessLogger() {
  const sessionIdRef = useRef<string>(crypto.randomUUID());

  useEffect(() => {
    const log = {
      id: crypto.randomUUID(),
      session_id: sessionIdRef.current,
      accessed_at: new Date().toISOString(),
    };
    fetch(`${baseUrl}/api/log/access`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(log),
    });
    fetch(`${baseUrl}/api/test-data/initialize`, { method: 'POST' }).catch(() => {});
    fetch(`${baseUrl}/api/test-data/seed`, { method: 'POST' }).catch(() => {});
  }, []);

  return null;
}
