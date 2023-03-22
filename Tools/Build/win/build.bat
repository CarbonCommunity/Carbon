::
:: Copyright (c) 2022-2023 Carbon Community 
:: All rights reserved
::
@echo off
set BASE=%~dp0

echo ** Get the base path of the script
pushd %BASE%..\..\..
set ROOT=%CD%
popd

if "%1" EQU "" (
	set TARGET=Debug
) else (
	set TARGET=%1
)

echo ** Set the build target config to %TARGET%

echo ** Cleanup the release folder
rmdir /s /q "%ROOT%\Release\.tmp\%TARGET%" 2>NUL
del /q "%ROOT%\Release\Carbon.%TARGET%.zip" 2>NUL

echo ** Build the solution
dotnet restore "%ROOT%\Carbon.Core" -v:m --nologo || exit /b
dotnet   clean "%ROOT%\Carbon.Core" -v:m --configuration %TARGET% --nologo || exit /b
dotnet   build "%ROOT%\Carbon.Core" -v:m --configuration %TARGET% --no-restore --no-incremental /p:UserConstants="%DEFINES%" || exit /b

echo ** Copy operating system specific files
echo "%TARGET%" | findstr /C:"Unix" >NUL && (
	copy /y "%ROOT%\Tools\Helpers\environment.sh"                 "%ROOT%\Release\.tmp\%TARGET%\carbon\tools\"
	copy /y "%ROOT%\Tools\UnityDoorstop\linux\x64\libdoorstop.so" "%ROOT%\Release\.tmp\%TARGET%"
	(CALL )
) || (                                                                                                                          
	copy /y "%ROOT%\Tools\Helpers\doorstop_config.ini"            "%ROOT%\Release\.tmp\%TARGET%"                                
	copy /y "%ROOT%\Tools\UnityDoorstop\windows\x64\doorstop.dll" "%ROOT%\Release\.tmp\%TARGET%\winhttp.dll"    
	(CALL )
)

echo "%TARGET%" | findstr /C:"Unix" >NUL && (
	echo "%TARGET%" | findstr /C:"Debug" >NUL && (
		set TOS=Linux
		set TAG=Debug
		(CALL )
	) || (                                                                                                                          
		set TOS=Linux
		set TAG=Release
		(CALL )
	)
	(CALL )
) || (                                                                                                                          
	echo "%TARGET%" | findstr /C:"Debug" >NUL && (
		set TOS=Windows
		set TAG=Debug
		(CALL )
	) || (                                                                                                                          
		set TOS=Windows
		set TAG=Release
		(CALL )
	)
	(CALL )
)

if "%2" NEQ "--no-archive" (
	echo ** Create the compressed archive 'Carbon.%TOS%.%TAG%.zip'
	powershell -Command "Compress-Archive -Update -Path '%ROOT%\Release\.tmp\%TARGET%\*' -DestinationPath '%ROOT%\Release\Carbon.%TOS%.%TAG%.zip'"
	"%ROOT%\Tools\BuildInfo\Carbon.BuildInfo.exe" -carbon "%ROOT%\Release\.tmp\%TARGET%\carbon\managed\Carbon" -o "%ROOT%\Release\Carbon.%TOS%.%TAG%.info"
)