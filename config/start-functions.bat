@echo off
setlocal enabledelayedexpansion

:: ========= パス定義 =========
set SCRIPT_DIR=%~dp0
set APP_DIR=%SCRIPT_DIR%..\src\Application
set FRONT_DIR=%SCRIPT_DIR%..\src\Presentation

:: ========= ポート解放 =========
for /f "tokens=5" %%a in ('netstat -aon ^| find ":7071" ^| find "LISTENING"') do taskkill /PID %%a /F >nul 2>nul
for /f "tokens=5" %%a in ('netstat -aon ^| find ":3000" ^| find "LISTENING"') do taskkill /PID %%a /F >nul 2>nul

:: ========= 必要なCLIの存在確認 =========
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

:: ========= local.settings.json / seed コピー =========
if exist "%SCRIPT_DIR%local.settings.json" copy /Y "%SCRIPT_DIR%local.settings.json" "%APP_DIR%\local.settings.json"
if exist "%SCRIPT_DIR%seed" xcopy /E /Y /I "%SCRIPT_DIR%seed" "%APP_DIR%\seed" >nul

:: ========= Functions 起動 =========
start "API" cmd /k "cd /d %APP_DIR% && func start"

:: ========= ポート7071の待機 =========
echo Waiting for Azure Functions (port 7071)...
:waitLoop
timeout /t 2 >nul
netstat -ano | findstr :7071 >nul
if errorlevel 1 goto waitLoop

:: ========= Frontend 起動 =========
set API_URL=http://localhost:7071
start "FRONT" cmd /k "cd /d %FRONT_DIR% && npm run dev"
