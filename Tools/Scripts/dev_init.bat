@echo off

set SCRIPTPATH=%~dp0..\..

rem Inits and downloads the submodules
git submodule init
git submodule update

rem Build Steam Downloading Utility
dotnet restore %SCRIPTPATH%\Tools\DepotDownloader
dotnet build %SCRIPTPATH%\Tools\DepotDownloader --no-incremental -c Release

rem Build .NET Assembly stripper, publicizer and general utility tool
dotnet restore %SCRIPTPATH%\Tools\NStrip
dotnet build %SCRIPTPATH%\Tools\NStrip --no-incremental -c Release

rem Keeping Unity DoorStop out of the game for now due to the more complex
rem build process.

rem Download rust binary libs
call %~dp0\dev_update.bat