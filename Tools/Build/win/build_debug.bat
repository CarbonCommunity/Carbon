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
set TARGET=Debug

rem Build the solution
dotnet restore "%ROOT%\Carbon.Core" --nologo || exit /b
dotnet   clean "%ROOT%\Carbon.Core" --configuration %TARGET% --nologo || exit /b
dotnet   clean "%ROOT%\Carbon.Core" --configuration %TARGET%Unix --nologo || exit /b
dotnet   build "%ROOT%\Carbon.Core" --configuration %TARGET% --no-restore --no-incremental || exit /b
dotnet   build "%ROOT%\Carbon.Core" --configuration %TARGET%Unix --no-restore --no-incremental || exit /b

rem Copy doorstop helper files
copy "%ROOT%\Tools\Helpers\doorstop_config.ini" "%ROOT%\Release\.tmp\%TARGET%" /y
copy "%ROOT%\Tools\UnityDoorstop\windows\x64\doorstop.dll" "%ROOT%\Release\.tmp\%TARGET%\winhttp.dll" /y

rem Copy doorstop helper files (unix)
copy "%ROOT%\Tools\Helpers\publicizer.sh" "%ROOT%\Release\.tmp\%TARGET%Unix\carbon\tools\" /y

rem Create the release archive file
del /q "%ROOT%\Release\*.zip"
powershell -Command "Compress-Archive -Update -Path '%ROOT%\Release\.tmp\%TARGET%\*' -DestinationPath '%ROOT%\Release\Carbon.%TARGET%.zip'"
powershell -Command "Compress-Archive -Update -Path '%ROOT%\Release\.tmp\%TARGET%Unix\*' -DestinationPath '%ROOT%\Release\Carbon.%TARGET%Unix.zip'"
