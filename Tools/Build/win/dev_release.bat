@echo off

::  ______ _______ ______ ______ _______ _______ 
:: |      |   _   |   __ \   __ \       |    |  |
:: |   ---|       |      <   __ <   -   |       |
:: |______|___|___|___|__|______/_______|__|____|
::         github.com/Carbon-Modding/Carbon.Core

set BASE=%~dp0

pushd %BASE%..\..\..
set ROOT=%CD%
popd

rem Build the solution
dotnet clean %ROOT%\Carbon.Core -t:Cleanup --configuration Release
dotnet build %ROOT%\Carbon.Core --configuration Release --no-incremental
dotnet build %ROOT%\Carbon.Core --configuration ReleaseUnix --no-incremental

"%ROOT%\Carbon.Core\Carbon.Patch\bin\Release\net48\Carbon.Patch.exe" --path %ROOT%
