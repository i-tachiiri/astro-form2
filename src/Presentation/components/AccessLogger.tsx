'use client';
import { useEffect, useRef } from 'react';

const baseUrl = process.env.NEXT_PUBLIC_API_BASE_URL ?? '';

export default function AccessLogger() {
  const sessionIdRef = useRef<string>(crypto.randomUUID());

  useEffect(() => {
    async function sendLogs() {
      const log = {
        id: crypto.randomUUID(),
        session_id: sessionIdRef.current,
        accessed_at: new Date().toISOString(),
      };
      try {
        await fetch(`${baseUrl}/api/log/access`, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(log),
        });
      } catch (err) {
        console.error('failed to log access', err);
      }
      fetch(`${baseUrl}/api/test-data/initialize`, { method: 'POST' }).catch(() => {});
      fetch(`${baseUrl}/api/test-data/seed`, { method: 'POST' }).catch(() => {});
    }

    sendLogs();
  }, []);

  return null;
}
