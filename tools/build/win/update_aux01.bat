@echo off

call "%~dp0update.bat" aux01-staging
exit /b %ERRORLEVEL%
