- /docs/ai-review.md へ仕様に対する不足や矛盾、曖昧さを記載
- PR作成前に下記を実施し、テストが通ればPRを作成
  - カバレッジが足りなければテストを実装する
  - フロントエンドは Next.js (React) で構築されているため、UI テストは npm スクリプトを利用する
```
npx tsc --noEmit
npx next lint
npx next build
npx next dev


dotnet build astro-form2.sln -c Release
dotnet format --verify-no-changes
dotnet test astro-form2.sln --collect:"XPlat Code Coverage"

# アプリケーション層（70%以上必要）
coverlet ./src/Test/Application/bin/Release/net8.0/Application.Tests.dll \
  --target "dotnet" \
  --targetargs "test ./src/Test/Application/Application.Tests.csproj -c Release --no-build" \
  --format cobertura \
  --output ./TestResults/coverage-application.xml \
  --threshold 70 \
  --threshold-type line \
  --threshold-stat total

# ドメイン層（70%以上必要）
coverlet ./src/Test/Domain/bin/Release/net8.0/Domain.Tests.dll \
  --target "dotnet" \
  --targetargs "test ./src/Test/Domain/Domain.Tests.csproj -c Release --no-build" \
  --format cobertura \
  --output ./TestResults/coverage-domain.xml \
  --threshold 70 \
  --threshold-type line \
  --threshold-stat total

```
