@echo off
setlocal enabledelayedexpansion

:: ========= パス定義 =========
set SCRIPT_DIR=%~dp0
set APP_DIR=%SCRIPT_DIR%..\src\Application
set WASM_DIR=%SCRIPT_DIR%..\src\Presentation
set SRC_DIR=%SCRIPT_DIR%..\src

set FUNC_OUT=%APP_DIR%\bin\Release\net8.0\publish
set WASM_OUT=%WASM_DIR%\bin\Release\net8.0\publish

:: ========= swa CLI の存在確認 =========
where swa >nul 2>nul
if errorlevel 1 (
    echo [INFO] swa CLI が見つかりません。npm でインストールします...
    npm install -g @azure/static-web-apps-cli
    if errorlevel 1 (
        echo [ERROR] swa CLI のインストールに失敗しました。Node.js / npm が必要です。
        pause & exit /b 1
    )
    echo [INFO] swa CLI をインストールしました。
)

:: ========= Functions ビルド =========
echo === Azure Functions publish ===
cd /d "%APP_DIR%"
dotnet publish -c Release -o "%FUNC_OUT%"
if errorlevel 1 (
    echo [ERROR] Azure Functions のビルドに失敗しました。
    pause & exit /b 1
)

:: local.settings.json / seed を Functions publish へコピー
copy /Y "%SCRIPT_DIR%local.settings.json" "%FUNC_OUT%\local.settings.json"
xcopy /E /Y /I "%SCRIPT_DIR%seed" "%FUNC_OUT%\seed" >nul

:: ========= WASM ビルド =========
echo === WASM publish ===
cd /d "%WASM_DIR%"
dotnet publish -c Release -o "%WASM_OUT%"
if errorlevel 1 (
    echo [ERROR] WASM のビルドに失敗しました。
    pause & exit /b 1
)

:: ========= ポート 7071 を強制解放 =========
for /f "tokens=5" %%a in ('netstat -aon ^| find ":7071" ^| find "LISTENING"') do taskkill /PID %%a /F >nul 2>nul

:: ========= SWA 起動 =========
cd /d "%SRC_DIR%"
echo === Starting SWA ===
swa start "%WASM_OUT%\wwwroot" --api-location "%FUNC_OUT%"
pause
