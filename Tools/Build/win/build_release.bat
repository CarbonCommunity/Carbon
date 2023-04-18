@echo off
set BASE=%~dp0

call "%BASE%\build.bat" Release %1
call "%BASE%\build.bat" ReleaseUnix %1
