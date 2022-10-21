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
if "%1" EQU "" (
	set TARGET=Debug
) else (
	set TARGET=%1
)

rem Cleanup the release folder
rmdir /s /q "%ROOT%\Release\.tmp\%TARGET%"
del /q "%ROOT%\Release\Carbon.%TARGET%.zip"

rmdir /s /q "%ROOT%\Release\.tmp\%TARGET%Unix"
del /q "%ROOT%\Release\Carbon.%TARGET%Unix.zip"

rem Build the solution
dotnet restore "%ROOT%\Carbon.Core" --nologo || exit /b
dotnet   clean "%ROOT%\Carbon.Core" --configuration %TARGET% --nologo || exit /b
dotnet   clean "%ROOT%\Carbon.Core" --configuration %TARGET%Unix --nologo || exit /b
dotnet   build "%ROOT%\Carbon.Core" --configuration %TARGET% --no-restore --no-incremental || exit /b
dotnet   build "%ROOT%\Carbon.Core" --configuration %TARGET%Unix --no-restore --no-incremental || exit /b

rem Copy doorstop helper files (windows)
copy "%ROOT%\Tools\Helpers\doorstop_config.ini" "%ROOT%\Release\.tmp\%TARGET%" /y  > NUL
copy "%ROOT%\Tools\UnityDoorstop\windows\x64\doorstop.dll" "%ROOT%\Release\.tmp\%TARGET%\winhttp.dll" /y  > NUL

rem Copy doorstop helper files (unix)
copy "%ROOT%\Tools\Helpers\publicizer.sh" "%ROOT%\Release\.tmp\%TARGET%Unix\carbon\tools\" /y > NUL

rem Create the zip archive release files
powershell -Command "Compress-Archive -Update -Path '%ROOT%\Release\.tmp\%TARGET%\*' -DestinationPath '%ROOT%\Release\Carbon.%TARGET%.zip'"
powershell -Command "Compress-Archive -Update -Path '%ROOT%\Release\.tmp\%TARGET%Unix\*' -DestinationPath '%ROOT%\Release\Carbon.%TARGET%Unix.zip'"
