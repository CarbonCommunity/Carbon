@echo off

call "%~dp0build.bat" Minimal MINIMAL edge_build -noarchive %*
exit /b %ERRORLEVEL%
