::
:: Copyright (c) 2022-2023 Carbon Community 
:: All rights reserved
::
@echo off

echo ** Get the base path of the script
pushd %~dp0..\..\..
set BUILD_ROOT=%CD%
popd

if "%1" EQU "" (
	set BUILD_TARGET=Debug
) else (
	set BUILD_TARGET=%1
)

echo ** Cleanup the release folder
rmdir /s /q "%BUILD_ROOT%\Release\.tmp\client\%BUILD_TARGET%" 2>NUL
del /q "%BUILD_ROOT%\Release\Carbon.Client.%BUILD_TARGET%.zip" 2>NUL

if "%DEFINES%" EQU "" (
	set DEFINES=%2
)

if "%DEFINES%" EQU "" (
	echo ** No defines.
) else (
	echo ** Defines: %DEFINES%
)

echo ** Build the solution
dotnet restore "%BUILD_ROOT%\Carbon.Core\Carbon.Components\Carbon.Client\CarbonCommunity.sln" -v:m --nologo || exit /b
dotnet   clean "%BUILD_ROOT%\Carbon.Core\Carbon.Components\Carbon.Client\CarbonCommunity.sln" -v:m --configuration %BUILD_TARGET% --nologo || exit /b
dotnet   build "%BUILD_ROOT%\Carbon.Core\Carbon.Components\Carbon.Client\CarbonCommunity.sln" -v:m --configuration %BUILD_TARGET% --no-restore --no-incremental ^
	/p:UserConstants=\"%2\" /p:UserVersion="%VERSION%" || exit /b

set CLIENT=%BUILD_ROOT%\Carbon.Core\Carbon.Components\Carbon.Client
set TOOLS=%CLIENT%\.tools
set INPUT=%CLIENT%\bin\%BUILD_TARGET%\net48
set OUTPUT=%BUILD_ROOT%\Release\.tmp\client\%BUILD_TARGET%

echo Create post-build structure
	mkdir "%OUTPUT%\BepInEx\plugins"
	"%TOOLS%\confuser\Confuser.CLI.exe" "%CLIENT%\Protect_%BUILD_TARGET%.crproj" || exit /b
	copy /y "%INPUT%\CarbonCommunity.Client.dll" "%OUTPUT%\BepInEx\plugins\CarbonCommunity.Client.dll"
	copy /y "%INPUT%\BouncyCastle.Crypto.dll" "%OUTPUT%\BepInEx\core\BouncyCastle.Crypto.dll"
	xcopy "%CLIENT%\.env\BepInEx" "%OUTPUT%" /E /H /C /I

if "%2" NEQ "--no-archive" (
	echo ** Create the compressed archive 'Carbon.Client.%BUILD_TARGET%.zip'
	powershell -Command "Compress-Archive -Update -Path '%OUTPUT%\*' -DestinationPath '%BUILD_ROOT%\Release\Carbon.Client.%BUILD_TARGET%.zip'"
)
