@echo off

::  ______ _______ ______ ______ _______ _______ 
:: |      |   _   |   __ \   __ \       |    |  |
:: |   ---|       |      <   __ <   -   |       |
:: |______|___|___|___|__|______/_______|__|____|
::         github.com/Carbon-Modding/Carbon.Core

set BASE=%~dp0

rem Get the base path of the script
pushd %BASE%..\..\..
set ROOT=%CD%
popd

rem Get the build target argument
if "%1" EQU "" (
	set TARGET=Debug
) else (
	set TARGET=%1
)

rem Build the solution
dotnet restore %ROOT%\Carbon.Core --nologo
dotnet   clean %ROOT%\Carbon.Core --configuration %TARGET% --nologo
dotnet   build %ROOT%\Carbon.Core --configuration %TARGET% --no-restore --no-incremental
dotnet   build %ROOT%\Carbon.Core --configuration %TARGET%Unix --no-restore --no-incremental

rem Create the patch file(s)
%ROOT%\Carbon.Core\Carbon.Patch\bin\%TARGET%\net48\Carbon.Patch.exe --path %ROOT% --configuration %TARGET%
