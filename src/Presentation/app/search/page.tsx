'use client';
import { useState, useRef } from 'react';
import Layout from '../../components/Layout';
import Header from '../../components/Header';
import PlaceSearchForm from '../../components/PlaceSearchForm';
import PlaceSearchResult from '../../components/PlaceSearchResult';

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
  const sessionIdRef = useRef<string>(crypto.randomUUID());

  return (
    <Layout>
      <Header />
      <PlaceSearchForm onSelected={setResult} sessionId={sessionIdRef.current} />
      <PlaceSearchResult result={result} />
    </Layout>
  );
}
