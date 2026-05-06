@echo off

call "%~dp0update.bat" release
exit /b %ERRORLEVEL%
