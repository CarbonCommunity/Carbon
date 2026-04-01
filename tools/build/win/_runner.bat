@echo off

if "%~1"=="" (
  echo Usage: %~nx0 ^<runner-file^> [args...]
  exit /b 1
)

for %%I in ("%~dp0..\..\..") do set "ROOT=%%~fI"

pushd "%ROOT%" >nul || exit /b 1
dotnet run --project "%ROOT%\src\Carbon.Tools\Carbon.Runner" %*
set "EXIT_CODE=%ERRORLEVEL%"
popd >nul

exit /b %EXIT_CODE%
