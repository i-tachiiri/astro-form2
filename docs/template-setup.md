```cmd
# 要管理者権限のコマンドプロンプト
npm uninstall -g azure-functions-core-tools

dotnet new blazorwasm -o src\Presentation -n Presentation
dotnet new classlib -o src\Shared -n Shared
dotnet new func --worker-runtime dotnet-isolated -o src\Application -n Application
dotnet new classlib -o src\Domain -n Domain
dotnet new classlib -o src\Infrastructure -n Infrastructure

dotnet new xunit -o src\Test\Application -n Application.Tests
dotnet new xunit -o src\Test\Domain -n Domain.Tests
dotnet new xunit -o src\Test\Infrastructure -n Infrastructure.Tests
dotnet new xunit -o src\Test\Presentation -n Presentation.Tests
dotnet new xunit -o src\Test\Shared -n Shared.Tests

dotnet add src\Test\Application\Application.Tests.csproj reference src\Application\Application\Application.csproj
dotnet add src\Test\Domain\Domain.Tests.csproj reference src\Domain\Domain.csproj
dotnet add src\Test\Infrastructure\Infrastructure.Tests.csproj reference src\Infrastructure\Infrastructure.csproj
dotnet add src\Test\Presentation\Presentation.Tests.csproj reference src\Presentation\Presentation.csproj
dotnet add src\Test\Shared\Shared.Tests.csproj reference src\Shared\Shared.csproj
```

.editorconfig
```
root = true

[*.cs]
indent_style = space
indent_size = 4
charset = utf-8

csharp_new_line_before_open_brace = all

# treat warnings as errors
warn_as_error = true
dotnet_diagnostic.CA1303.severity = error
```

.gitignore
```
# ========================
# .NET ビルド成果物
# ========================
bin/
obj/
*.dll
*.exe
*.pdb
*.app/

# ========================
# Visual Studio / VSCode
# ========================
.vscode/
*.user
*.suo

# ========================
# Mac / Windows / Linux OS由来
# ========================
.DS_Store
Thumbs.db
ehthumbs.db
desktop.ini

# ========================
# パッケージ・一時ファイル
# ========================
packages/
packages-microsoft-prod.deb
*.nupkg
*.tar.gz
*.zip

# ========================
# Azure Functions 実行構成
# ========================
local.settings.json

# ========================
# テスト出力
# ========================
TestResults/
coverage*.json
coverage*.xml
*.trx
*.coverage
*.log

# ========================
# dotnet user secrets（開発環境限定）
# ========================
secrets.json

# ========================
# Rider / JetBrains 系（必要に応じて）
# ========================
.idea/
*.sln.iml

# ========================
# npm / JS 系（Next.js などを利用する場合）
# ========================
node_modules/
dist/
.env
.env.*

```
readme.md
```md
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
```