@echo off

setlocal
set "SUPERROOT=%~dp0Tools\Build\win"

pushd "%SUPERROOT%"
call bootstrap.bat
call build_debug_noarchive.bat
popd

endlocal
