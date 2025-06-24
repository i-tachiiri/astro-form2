# 日本語で記載してください

- Cosmos DB は `/admin/initialize` API で手動作成する設計だったが、起動時にスキーマを自動作成する HostedService `CosmosDbInitializer` を追加
- フロントエンドを Blazor から Next.js (React) に変更
- 仕様では `next export` を前提とした静的サイトとして構築する記載があるが、実装
  では API への動的アクセスが必要なため `next.config.mjs` の `output: 'export'`
  設定を削除し、動的レンダリングに対応した
- `SearchResults` と `PlaceDetails` の JSON プロパティ名は実装では camelCase
  (`placeId`, `mapUrl` 等) だが、仕様書では snake_case (`place_id`, `map_url` 等)
  と記載されている
