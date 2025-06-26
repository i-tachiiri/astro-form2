@echo off
setlocal enabledelayedexpansion

:: ========= パス定義 =========
set SCRIPT_DIR=%~dp0
set APP_DIR=%SCRIPT_DIR%..\src\Application
set FRONT_DIR=%SCRIPT_DIR%..\src\Presentation

:: ========= 最新版コードの取得 =========
cd ..\
git reset --hard
git pull origin main
cd %SCRIPT_DIR%

:: ========= ポート解放 =========
for /f "tokens=5" %%a in ('netstat -aon ^| find ":7071" ^| find "LISTENING"') do taskkill /PID %%a /F >nul 2>nul
for /f "tokens=5" %%a in ('netstat -aon ^| find ":3000" ^| find "LISTENING"') do taskkill /PID %%a /F >nul 2>nul

:: ========= CLIの存在確認 =========
where func >nul 2>nul
if errorlevel 1 (
    echo [ERROR] Azure Functions Core Tools が見つかりません。インストールしてください。
    pause & exit /b 1
)

where npm >nul 2>nul
if errorlevel 1 (
    echo [ERROR] Node.js / npm が見つかりません。インストールしてください。
    pause & exit /b 1
)

:: ========= 設定ファイルとシードコピー =========
if exist "%SCRIPT_DIR%local.settings.json" copy /Y "%SCRIPT_DIR%local.settings.json" "%APP_DIR%\local.settings.json"
if exist "%SCRIPT_DIR%seed" xcopy /E /Y /I "%SCRIPT_DIR%seed" "%APP_DIR%\seed" >nul
if exist "%SCRIPT_DIR%.env.local" copy /Y "%FRONT_DIR%\.env.local"

:: ========= Functions 並列起動 =========
start "API" cmd /k "cd /d %APP_DIR% && func start"

:: ========= Frontend 並列起動 =========
set API_URL=http://localhost:7071
start "FRONT" cmd /k "cd /d %FRONT_DIR% && npm run dev"

:: ========= 両ポートの起動を待機 =========
echo Waiting for both ports (3000 and 7071)...
:waitPorts
timeout /t 2 >nul
set PORT3000=0
set PORT7071=0

netstat -ano | findstr :3000 >nul && set PORT3000=1
netstat -ano | findstr :7071 >nul && set PORT7071=1

if not "!PORT3000!"=="1" goto waitPorts
if not "!PORT7071!"=="1" goto waitPorts

:: ========= ブラウザ起動 =========
echo Both ports are ready. Launching browser...
start "" "http://localhost:3000"
