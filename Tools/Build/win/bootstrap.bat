@echo off

echo   ______ _______ ______ ______ _______ _______ 
echo  ^|      ^|   _   ^|   __ \   __ \       ^|    ^|  ^|
echo  ^|   ---^|       ^|      ^<   __ ^<   -   ^|       ^|
echo  ^|______^|___^|___^|___^|__^|______/_______^|__^|____^|
echo                         discord.gg/eXPcNKK4yd
echo.

set BASE=%~dp0

pushd %BASE%..\..\..
set ROOT=%CD%
popd

rem Inits and downloads the submodules
git submodule init
git submodule update

rem Build Steam Downloading Utility
dotnet restore %ROOT%\Tools\DepotDownloader --nologo --force
dotnet clean   %ROOT%\Tools\DepotDownloader --configuration Release --nologo
dotnet build   %ROOT%\Tools\DepotDownloader --configuration Release --no-restore --no-incremental

rem Build .NET Assembly stripper, publicizer and general utility tool
dotnet restore %ROOT%\Tools\NStrip --nologo --force
dotnet clean   %ROOT%\Tools\NStrip --configuration Release --nologo
dotnet build   %ROOT%\Tools\NStrip --configuration Release --no-restore --no-incremental

rem Keeping Unity DoorStop out of the game for now due to the more complex
rem build process.

rem Download rust binary libs
call %BASE%\update.bat