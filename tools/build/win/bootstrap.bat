@echo off

set "TARGET=%~1"
if not defined TARGET set "TARGET=release"

call "%~dp0_runner.bat" tools/build/runners/bootstrap.cs || exit /b %ERRORLEVEL%
call "%~dp0_runner.bat" tools/build/runners/update.cs "%TARGET%"
exit /b %ERRORLEVEL%
