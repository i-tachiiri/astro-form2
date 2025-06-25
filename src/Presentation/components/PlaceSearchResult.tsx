import { Table, TableHeader, TableColumn, TableBody, TableRow, TableCell } from '@nextui-org/table';

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
    <Table aria-label="search result" hideHeader={false} className="w-full">
      <TableHeader>
        <TableColumn>項目名</TableColumn>
        <TableColumn>検索結果</TableColumn>
      </TableHeader>
      <TableBody>
        <TableRow key="address">
          <TableCell>住所</TableCell>
          <TableCell>{result.address}</TableCell>
        </TableRow>
        <TableRow key="lat">
          <TableCell>緯度</TableCell>
          <TableCell>{result.lat}</TableCell>
        </TableRow>
        <TableRow key="lng">
          <TableCell>経度</TableCell>
          <TableCell>{result.lng}</TableCell>
        </TableRow>
        <TableRow key="map">
          <TableCell>GoogleMap</TableCell>
          <TableCell><a href={result.map_url} target="_blank">{result.map_url}</a></TableCell>
        </TableRow>
      </TableBody>
    </Table>
  );
}
