@echo off

set "BRANCH=%~1"
set "OUTPUT=%~2"

if not defined BRANCH set "BRANCH=release"
if not defined OUTPUT set "OUTPUT=xx"

call "%~dp0_runner.bat" tools/build/runners/patcher_setup.cs "%BRANCH%" "%OUTPUT%"
exit /b %ERRORLEVEL%
