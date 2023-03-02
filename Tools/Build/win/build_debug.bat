@echo off
set BASE=%~dp0

call "%BASE%\build.bat" Debug %1
call "%BASE%\build.bat" DebugUnix %1
