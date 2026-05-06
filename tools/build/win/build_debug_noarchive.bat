@echo off

call "%~dp0build.bat" Debug EDGE edge_build -noarchive %*
exit /b %ERRORLEVEL%
