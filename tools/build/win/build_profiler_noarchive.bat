@echo off

call "%~dp0build_native.bat" || exit /b %ERRORLEVEL%

if not defined VERSION set "VERSION=2.0.0"

call "%~dp0_runner.bat" tools/build/runners/profiler.cs Debug HARMONYMOD edge_build -noarchive %* || exit /b %ERRORLEVEL%
call "%~dp0_runner.bat" tools/build/runners/profiler.cs DebugUnix HARMONYMOD edge_build -noarchive %*
exit /b %ERRORLEVEL%
