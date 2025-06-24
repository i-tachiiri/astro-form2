## 仕様書

- [API仕様書](specification/api.yaml)  
- [データ構造](specification/data-design.yaml)
- - [データ構造](specification/project.yaml)  
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
- [Presentation](src/Presentation) – UIレイヤー（Next.js）
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

## 開発環境のセットアップ

`specification/environment.yaml` の内容に合わせ、Bicep テンプレートとパラメータ
ファイルを `infra/` 配下に配置しています。

- `infra/main.bicap` – module 形式で各サービスを展開するメインテンプレート
- `infra/modules/` – Static Web Apps、Functions、Key Vault、Cosmos DB 用モジュール
- `infra/parameters/local.json` – ローカル開発用パラメータ
- `infra/parameters/dev.json` – 開発環境用パラメータ
- `infra/parameters/prod.json` – 本番環境用パラメータ

ローカル環境では Azure リソースを作成せず、`npm run dev` や `func start` でアプリを
起動し、データベースには Cosmos DB Emulator を利用します。

`local.settings.sample.json` を `local.settings.json` にコピーし、
`GooglePlacesApiKey` と `CosmosDbConnection` を設定してから実行してください。
