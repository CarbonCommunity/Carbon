@echo off

set SCRIPTPATH=%~dp0..\..

dotnet restore %SCRIPTPATH%\Carbon.Core\
dotnet restore %SCRIPTPATH%\Carbon.Core\Carbon
dotnet restore %SCRIPTPATH%\Carbon.Core\Carbon.Doorstop
dotnet restore %SCRIPTPATH%\Carbon.Core\Carbon.Patch

dotnet build %SCRIPTPATH%\Carbon.Core\Carbon -c ReleaseUnix
dotnet build %SCRIPTPATH%\Carbon.Core\Carbon -c Release
dotnet build %SCRIPTPATH%\Carbon.Core\Carbon.Doorstop -c Release
dotnet build %SCRIPTPATH%\Carbon.Core\Carbon.Patch -c Release

"%SCRIPTPATH%\Carbon.Core\Carbon.Patch\bin\Release\net48\Carbon.Patch.exe"
