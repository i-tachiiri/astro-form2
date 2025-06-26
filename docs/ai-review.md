# 日本語で記載してください

## 仕様と実装の差異

- **フロントエンドのビルド方式**
  - 仕様では `next build && next export` を実行し `out/` に出力する想定【F:specification/project.yaml†L1-L7】。
  - 実装の `package.json` では `next build` のみで静的書き出しを行っていない【F:src/Presentation/package.json†L4-L7】。
- **API の認証方式**
  - 仕様では Microsoft Entra ID を利用する【F:specification/project.yaml†L9-L15】。
  - 実装では Azure Functions の `AuthorizationLevel.Anonymous` を使用し認証を行っていない【F:src/Application/Functions/MapFunctions.cs†L24-L27】。
- **`search_result` ログの項目不足**
  - 仕様のデータ設計では `place_name` を保存すると定義【F:specification/data-design.yaml†L57-L68】。
  - 実装の `SearchResultLog` には該当プロパティが存在しない【F:src/Domain/Models/SearchResultLog.cs†L6-L33】。
- **フォント設定の相違**
  - 仕様は `Inter` と `Noto Sans JP` を用いる【F:specification/ui-design.yaml†L1-L5】。
  - 実装のグローバル CSS では `Segoe UI` を指定している【F:src/Presentation/globals.css†L4-L9】。
- **アイコンとモーションの未実装**
  - 仕様では `@fluentui/react-icons` の利用および `fade`/`slide` のモーションを想定【F:specification/ui-design.yaml†L5-L10】。
  - 実装にはそれらを使用したコードが存在しない。
- **レスポンシブ対応の不足**
  - 仕様のブレークポイントは `480px, 640px, 768px, 1024px, 1280px`【F:specification/ui-design.yaml†L27-L31】。
  - 実装の CSS では `480px` と `768px` のみ定義されている【F:src/Presentation/globals.css†L20-L25】。
- **検索フォームの未定義挙動**
  - 仕様では検索候補件数や空結果時のメッセージについて言及がない【F:specification/ui-design.yaml†L77-L81】。
  - 実装では候補がない場合 `No results found.` と表示され、候補は1件のみだった【F:src/Presentation/components/PlaceSearchForm.tsx†L46-L54】。

