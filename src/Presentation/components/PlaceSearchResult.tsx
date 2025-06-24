interface PlaceDetails {
  place_id: string;
  name: string;
  address: string;
  lat: number;
  lng: number;
  map_url: string;
}

export default function PlaceSearchResult({ result }: { result?: PlaceDetails }) {
  if (!result) return null;
  return (
    <table>
      <thead>
        <tr><th>項目名</th><th>検索結果</th></tr>
      </thead>
      <tbody>
        <tr><td>住所</td><td>{result.address}</td></tr>
        <tr><td>緯度</td><td>{result.lat}</td></tr>
        <tr><td>経度</td><td>{result.lng}</td></tr>
        <tr><td>GoogleMap</td><td><a href={result.map_url} target="_blank">{result.map_url}</a></td></tr>
      </tbody>
    </table>
  );
}
