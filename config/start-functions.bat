@echo off
setlocal

echo [INFO] Copying local.settings.json to src\Application...
copy /Y "%~dp0local.settings.json" "..\src\Application\local.settings.json"

echo [INFO] Starting Azure Functions...
cd ..\src\Application
func start

endlocal
