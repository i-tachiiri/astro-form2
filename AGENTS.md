- 仕様と指示が相反する場合、実装を行わずに、差異を /docs/ai-review.md へ記載
- PR作成前に下記を実施し、テストが通ればPRを作成
```
dotnet build astro-form2.sln -c Release
dotnet format --verify-no-changes
dotnet test astro-form2.sln --collect:"XPlat Code Coverage"
coverlet ./YourTestProject/bin/Release/net8.0/YourTestProject.dll --target "dotnet" --targetargs "test ./YourTestProject/YourTestProject.csproj" --format cobertura
```


