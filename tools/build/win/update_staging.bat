@echo off

call "%~dp0update.bat" staging
exit /b %ERRORLEVEL%
