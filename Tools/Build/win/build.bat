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

rem Build the solution
dotnet restore "%ROOT%\Carbon.Core" -v:m --nologo || exit /b
dotnet   clean "%ROOT%\Carbon.Core" -v:m --configuration %TARGET% --nologo || exit /b
dotnet   build "%ROOT%\Carbon.Core" -v:m --configuration %TARGET% --no-restore --no-incremental || exit /b

rem Copy doorstop helper files (windows)
copy /y "%ROOT%\Tools\Helpers\doorstop_config.ini" "%ROOT%\Release\.tmp\%TARGET%" > NUL
copy /y "%ROOT%\Tools\UnityDoorstop\windows\x64\doorstop.dll" "%ROOT%\Release\.tmp\%TARGET%\winhttp.dll" > NUL

echo "%TARGET%" | findstr /C:"Unix" >NUL && (
	rem Copy doorstop helper files (unix)
	copy /y "%ROOT%\Tools\Helpers\environment.sh" "%ROOT%\Release\.tmp\%TARGET%\carbon\tools\" > NUL
	copy /y "%ROOT%\Tools\Helpers\publicizer.sh" "%ROOT%\Release\.tmp\%TARGET%\carbon\tools\" > NUL
)

rem Create the standalone files
copy /y "%ROOT%\Release\.tmp\%TARGET%\HarmonyMods\Carbon.Loader.dll" "%ROOT%\Release"
copy /y "%ROOT%\Release\.tmp\%TARGET%\carbon\managed\Carbon.dll" "%ROOT%\Release"
copy /y "%ROOT%\Release\.tmp\%TARGET%\carbon\managed\Carbon.Doorstop.dll" "%ROOT%\Release"

rem Create the zip archive release files
powershell -Command "Compress-Archive -Update -Path '%ROOT%\Release\.tmp\%TARGET%\*' -DestinationPath '%ROOT%\Release\Carbon.%TARGET%.zip'"
