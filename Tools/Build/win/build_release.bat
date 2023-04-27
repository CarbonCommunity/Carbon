@echo off
call "%~dp0\build.bat" Release %1
call "%~dp0\build.bat" ReleaseUnix %1
