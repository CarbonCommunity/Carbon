@echo off

rem Carbon with Oxide: also ships the Oxide compatibility package and the generated Oxide hooks
call "%~dp0build.bat" Debug EDGE edge_build -oxide -noarchive %*
exit /b %ERRORLEVEL%
