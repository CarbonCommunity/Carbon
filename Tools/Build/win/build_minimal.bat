@echo off
call "%~dp0\build.bat" Minimal %1
call "%~dp0\build.bat" MinimalUnix %1
