@echo off

call "%~dp0build.bat" Release EDGE edge_build %*
exit /b %ERRORLEVEL%
