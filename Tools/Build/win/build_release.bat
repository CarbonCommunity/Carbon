@echo off
set BASE=%~dp0

call "%BASE%\build.bat" Release
call "%BASE%\build.bat" ReleaseUnix