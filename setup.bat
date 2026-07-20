@echo off

setlocal
set "BUILD_ROOT=%~dp0tools\build\win"

call "%BUILD_ROOT%\bootstrap.bat" || goto :done
call "%BUILD_ROOT%\build_debug_noarchive.bat" %*

:done
set "EXIT_CODE=%ERRORLEVEL%"
endlocal

exit /b %EXIT_CODE%
