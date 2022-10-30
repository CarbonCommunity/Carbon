@echo off
set BASE=%~dp0

call "%BASE%\build.bat" Debug
call "%BASE%\build.bat" DebugUnix
