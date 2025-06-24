@echo off
setlocal enabledelayedexpansion

:: パスの定義
set SCRIPT_DIR=%~dp0
set APP_DIR=%SCRIPT_DIR%..\src\Application
set WASM_DIR=%SCRIPT_DIR%..\src\Presentation
set SRC_DIR=%SCRIPT_DIR%..\src
set BIN_RELEASE=%APP_DIR%\bin\Release\net8.0
set WASM_OUT=%WASM_DIR%\bin\Release\net8.0\wwwroot
set FUNC_OUT=%APP_DIR%\bin\Release\net8.0\publish

:: local.settings.json / seed を各所にコピー
copy /Y "%SCRIPT_DIR%local.settings.json" "%APP_DIR%\local.settings.json"
xcopy /E /Y /I "%SCRIPT_DIR%seed" "%APP_DIR%\seed"
xcopy /E /Y /I "%SCRIPT_DIR%seed" "%BIN_RELEASE%\seed"
copy /Y "%SCRIPT_DIR%local.settings.json" "%BIN_RELEASE%\local.settings.json"
copy /Y "%SCRIPT_DIR%local.settings.json" "%FUNC_OUT%\local.settings.json"


:: swa コマンドの存在確認
where swa >nul 2>nul
if errorlevel 1 (
    echo [INFO] swa CLI が見つかりません。npm でインストールを試みます...
    npm install -g @azure/static-web-apps-cli
    if errorlevel 1 (
        echo [ERROR] swa CLI のインストールに失敗しました。Node.js / npm がインストールされているか確認してください。
        pause
        exit /b 1
    )
    echo [INFO] swa CLI のインストール完了。
)

:: Azure Functions ビルド（publish）
cd /d "%APP_DIR%"
echo === Building Azure Functions ===
dotnet publish -c Release -o "%FUNC_OUT%"
if errorlevel 1 (
    echo ERROR: Azure Functions のビルドに失敗しました。
    pause
    exit /b 1
)

:: WASM ビルド（publish）
cd /d "%WASM_DIR%"
echo === Building WASM ===
dotnet publish -c Release -o "%WASM_OUT%"
if errorlevel 1 (
    echo ERROR: WASM のビルドに失敗しました。
    pause
    exit /b 1
)

:: swa 起動
cd /d "%SRC_DIR%"
echo === Starting SWA ===
for /f "tokens=5" %%a in ('netstat -aon ^| find ":7071" ^| find "LISTENING"') do taskkill /PID %%a /F
swa start "%WASM_OUT%" --api-location "%FUNC_OUT%"
pause
