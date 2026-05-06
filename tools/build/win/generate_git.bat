@echo off

call "%~dp0_runner.bat" tools/build/runners/git_feedback.cs %*
exit /b %ERRORLEVEL%
