::
:: Copyright (c) 2022-2023 Carbon Community 
:: All rights reserved
::
@echo off

set HOME=%cd%
set API_KEY=%1
set VERSION=%2

cd "%~dp0\..\..\..\Carbon.Core"
dotnet pack -o .nugets -c Release /p:PackageVersion=%VERSION% --no-build
dotnet nuget push .nugets\Carbon.Community.%VERSION%.nupkg --api-key "%API_KEY%" --source "https://api.nuget.org/v3/index.json"

cd "%HOME%"