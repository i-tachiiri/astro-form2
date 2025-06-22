## 仕様書

- [API仕様書](specification/api-spec.yaml)  
- [システム構成](specification/architecture.yaml)  
- [データ構造](specification/data-design.yaml)  
- [システム環境](specification/environment.yaml)  
- [セキュリティ](specification/security.yaml)  
- [テスト・CI](specification/test-policy.yaml)  
- [UIデザイン・画面](specification/ui-design.yaml)  
- [ユースケース](specification/use-cases.yaml)

---

## ソースコード

### 本体（`src/`）

- [Application](src/Application) – ユースケースやAPIのエントリーポイント（Azure Functionsなど）
- [Domain](src/Domain) – 業務ロジック（Entity / ValueObject / ドメインサービス）
- [Infrastructure](src/Infrastructure) – データアクセスや外部API連携（CosmosDB, Blobなど）
- [Presentation](src/Presentation) – UIレイヤー（Blazor WASMなど）
- [Shared](src/Shared) – DTO / Enum / 共通モデル

### テスト（`src/Test/`）

- [Application Test](src/Test/Application) – Application層のテスト
- [Domain Test](src/Test/Domain) – Domain層のユニットテスト
- [Infrastructure Test](src/Test/Infrastructure) – 外部接続処理のテスト（Mock対応含む）
- [Presentation Test](src/Test/Presentation) – UIのテスト（BUnitなどを想定）
- [Shared Test](src/Test/Shared) – DTOやユーティリティのテスト

## ドキュメント

- [Ai Review](docs/ai-review.md) - AIペアコーディングで、AIが仕様との差異を確認し、記載する為のドキュメント
- [マニュアル](docs/human-manual/md) - 人間向けのAI指示案

## 環境設定

`infra/` ディレクトリには Bicep テンプレートを配置しています。環境ごとのパラメーター ファイルは以下の通りです。

- `infra/parameters/local.json` – ローカル開発向け
- `infra/parameters/dev.json` – 開発環境向け
- `infra/parameters/prod.json` – 本番環境向け

ローカル環境では Azure リソースを作成せず、`npm run dev` や `func start` によりアプリを起動します。Cosmos DB Emulator を利用してください。
