@echo off
call "%~dp0\build.bat" Debug %1
call "%~dp0\build.bat" DebugUnix %1
