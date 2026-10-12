@echo off

rem Carbon Vanilla: leaves out the Oxide compatibility package and the generated Oxide hooks
call "%~dp0build.bat" Minimal MINIMAL edge_build -vanilla %*
exit /b %ERRORLEVEL%
