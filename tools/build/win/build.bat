@echo off

call "%~dp0_runner.bat" tools/build/runners/build.cs %*
exit /b %ERRORLEVEL%
