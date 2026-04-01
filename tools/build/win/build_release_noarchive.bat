@echo off

call "%~dp0build.bat" Release EDGE edge_build -noarchive %*
exit /b %ERRORLEVEL%
