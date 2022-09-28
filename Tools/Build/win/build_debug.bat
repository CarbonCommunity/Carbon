@echo off

echo   ______ _______ ______ ______ _______ _______ 
echo  ^|      ^|   _   ^|   __ \   __ \       ^|    ^|  ^|
echo  ^|   ---^|       ^|      ^<   __ ^<   -   ^|       ^|
echo  ^|______^|___^|___^|___^|__^|______/_______^|__^|____^|
echo                         discord.gg/eXPcNKK4yd
echo.

set BASE=%~dp0

rem Get the base path of the script
pushd %BASE%..\..\..
set ROOT=%CD%
popd

rem Set the build target config
set TARGET=Debug

rem Build the solution + generate identifier
dotnet restore "%ROOT%\Carbon.Cor"e --nologo
dotnet   clean "%ROOT%\Carbon.Core" --configuration %TARGET% --nologo
dotnet   build "%ROOT%\Carbon.Core" --configuration %TARGET% --no-restore --no-incremental
dotnet   build "%ROOT%\Carbon.Core" --configuration %TARGET%Unix --no-restore --no-incremental

rem Build the solution for actual release
dotnet restore "%ROOT%\Carbon.Core" --nologo
dotnet   clean "%ROOT%\Carbon.Core" --configuration %TARGET% --nologo
dotnet   build "%ROOT%\Carbon.Core" --configuration %TARGET% --no-restore --no-incremental
dotnet   build "%ROOT%\Carbon.Core" --configuration %TARGET%Unix --no-restore --no-incremental

rem Create the patch file(s)
"%ROOT%\Carbon.Core\Carbon.Patch\bin\%TARGET%\net48\Carbon.Patch.exe" --path "%ROOT%" --configuration %TARGET%
