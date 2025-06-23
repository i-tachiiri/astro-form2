- 仕様と指示が相反する場合、実装を行わずに、差異を /docs/ai-review.md へ記載
- PR作成前に下記を実施し、テストが通ればPRを作成。通らなければ通るように修正
```
dotnet build astro-form2.sln -c Release
dotnet format --verify-no-changes
dotnet test astro-form2.sln --collect:"XPlat Code Coverage"

# アプリケーション層（70%以上必要）
coverlet ./src/Test/Application/bin/Release/net8.0/Application.Test.dll \
  --target "dotnet" \
  --targetargs "test ./src/Test/Application/Application.Test.csproj -c Release" \
  --format cobertura \
  --output ./TestResults/coverage-application.xml \
  --threshold 70 \
  --threshold-type line \
  --threshold-stat total

# ドメイン層（70%以上必要）
coverlet ./src/Test/Domain/bin/Release/net8.0/Domain.Test.dll \
  --target "dotnet" \
  --targetargs "test ./src/Test/Domain/Domain.Test.csproj -c Release" \
  --format cobertura \
  --output ./TestResults/coverage-domain.xml \
  --threshold 70 \
  --threshold-type line \
  --threshold-stat total

```
