::
:: Copyright (c) 2022 Carbon Community 
:: All rights reserved
::
@echo off

set BASE=%~dp0

rem Get the base path of the script
pushd %BASE%..\..\..
set ROOT=%CD%
popd

rem Set the build target config
set TARGET=Release

rem Build the solution
dotnet restore "%ROOT%\Carbon.Core" --nologo
dotnet   clean "%ROOT%\Carbon.Core" --configuration %TARGET% --nologo
dotnet   build "%ROOT%\Carbon.Core" --configuration %TARGET% --no-restore --no-incremental
dotnet   build "%ROOT%\Carbon.Core" --configuration %TARGET%Unix --no-restore --no-incremental

rem Create the patch file(s)
"%ROOT%\Carbon.Core\Carbon.Patch\bin\%TARGET%\net48\Carbon.Patch.exe" --path "%ROOT%" --configuration %TARGET%
