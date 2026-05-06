@echo off

call "%~dp0build.bat" Minimal MINIMAL edge_build %*
exit /b %ERRORLEVEL%
