# 日本語で記載してください

- Cosmos DB は `/admin/initialize` API で手動作成する設計だったが、起動時にスキーマを自動作成する HostedService `CosmosDbInitializer` を追加
- data-design.yaml の search_result スキーマでは `place_name` を保持するが、実装の SearchResultLog には存在しない
- ui-design.yaml で place_search_result コンポーネントは DataGrid を使用する設計だが、実装では単純な `<table>` で表示している
- environment.yaml では `infra/main.bicep` を想定しているが、リポジトリには `infra/main.bicap` と綴りが異なるファイルがある
- security.yaml では秘密情報を Key Vault から取得する設計だが、実装では環境変数から直接取得しており `KeyVaultService` は未使用
- test-policy では Serilog を用いるため `Console.WriteLine` を禁止しているが、`Program.cs` で `Console.Error.WriteLine` が残っている
