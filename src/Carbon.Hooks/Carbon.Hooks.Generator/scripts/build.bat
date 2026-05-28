@echo off

pushd %~dp0..
set WORKDIR=%CD%
popd

:: Update to match your Carbon protocol
set PROTOCOL=%3
set ID=Carbon.Hooks.Oxide

:: Update to match your local environment
set CARBON_SLN=%cd%\..\..\..
set CARBON_REDIST=%cd%\..\ftp
set RUST_OPJ=%4
set BOOTSTRAP=null
set VERSION=2.0.0

if "%PROTOCOL%" EQU "" (
	set /p PROTOCOL=Carbon Protocol: 
)

if "%1" EQU "" (
	set /p TARGET=Target: 
) else (
	set TARGET=%1
)

if "%2" EQU "" (
	set /p STEAM_TARGET=Steam Branch: 
) else (
	set STEAM_TARGET=%2
)

if "%STEAM_TARGET%" EQU "public" (
	set BOOTSTRAP=update
)

if "%STEAM_TARGET%" EQU "release" (
	set BOOTSTRAP=update_release
)

if "%STEAM_TARGET%" EQU "staging" (
	set BOOTSTRAP=update_staging
)

if "%STEAM_TARGET%" EQU "aux01" (
	set BOOTSTRAP=update_aux01
)

if "%STEAM_TARGET%" EQU "aux02" (
	set BOOTSTRAP=update_aux02
)

set TOOLS=%WORKDIR%\src
set MANAGED=%WORKDIR%\.rust

echo ** Initializing Carbon hook build for protocol %PROTOCOL%

echo ** Carbon Bootstrap (%BOOTSTRAP%.bat) [STEAM: %STEAM_TARGET%]
(
	set HERE=%cd%
	cd %CARBON_SLN%\..\
	echo %cd%
	dotnet run --project "src\Carbon.Tools\Carbon.Runner" "tools\build\runners\bootstrap.cs"
	dotnet run --project "src\Carbon.Tools\Carbon.Runner" "tools\build\runners\update.cs" %STEAM_TARGET%
	cd %HERE%
	
	if NOT "%RUST_OPJ%" EQU "" (
		echo ** Downloading OPJ file from '%RUST_OPJ%'
		curl -fSL -o "%MANAGED%\Rust.opj" "%RUST_OPJ%"
	) else (
		echo ** No OPJ endpoint, no downloading papi. Using local.
	)
)

echo ** Generate the hooks from the opj file
(
	del /q "%CARBON_SLN%\Carbon.Hooks\%ID%\src\*.cs"
	del /q "%CARBON_SLN%\Carbon.Hooks\%ID%\src\*.txt"
	dotnet run --project "%TOOLS%" ^
		--input "%MANAGED%\Rust.opj" ^
		--managed "%CARBON_SLN%\..\rust\windows\RustDedicated_Data\Managed" --output "%CARBON_SLN%\Carbon.Hooks\%ID%\src" || exit /b
)

FOR %%T IN (%TARGET%Unix %TARGET%) DO call :body %%T
goto :eof

:body
set WORKING_TARGET=%~1

if "%WORKING_TARGET%" EQU "%TARGET%" (
	set TARGET_OS=windows
) else (
	set TARGET_OS=linux
)

if "%WORKING_TARGET%" EQU "Debug" (
	set WORKING_TARGET_LOWER=debug
) else if "%WORKING_TARGET%" EQU "DebugUnix" (
	set WORKING_TARGET_LOWER=debugunix
) else if "%WORKING_TARGET%" EQU "Release" (
	set WORKING_TARGET_LOWER=release
) else if "%WORKING_TARGET%" EQU "ReleaseUnix" (
	set WORKING_TARGET_LOWER=releaseunix
)

set OUT_STEAM_LIBS=%CARBON_SLN%\..\rust\%TARGET_OS%
set OUT_CSFILES=%CARBON_SLN%\Carbon.Hooks\%ID%\src
set OUT_SLN_BIN=%CARBON_SLN%\..\release\.tmp\%WORKING_TARGET%\carbon\managed
set OUT_LOG=%CARBON_REDIST%\logs\log_%WORKING_TARGET_LOWER%_%STEAM_TARGET%.txt
set OUT_FTP=%CARBON_REDIST%\ftp_%WORKING_TARGET_LOWER%_%STEAM_TARGET%.txt

if NOT EXIST %CARBON_REDIST%\logs (
	mkdir %CARBON_REDIST%\logs
)

echo ---------------------------------------------------------------------------
echo   Configuration: %WORKING_TARGET% (%TARGET_OS%, %STEAM_TARGET%)
echo  Workdir folder: %WORKDIR%
echo    Tools folder: %TOOLS%
echo  Managed folder: %OUT_STEAM_LIBS%
echo ---------------------------------------------------------------------------

echo > %OUT_LOG%

echo ** Build carbon managed files
(
	set HERE=%cd%
	cd %CARBON_SLN%\..\
	dotnet run --project "src\Carbon.Tools\Carbon.Runner" "tools\build\runners\build.cs" %WORKING_TARGET% HOOKGEN
	cd %HERE%
) >> %OUT_LOG%

echo ** Patch rust managed libs
(
	dotnet run --project "%CARBON_SLN%\..\src\Carbon.Tools\Carbon.Publicizer" ^
		"%OUT_STEAM_LIBS%\RustDedicated_Data\Managed" || exit /b
) >> %OUT_LOG%

echo ** Build hook dll files
(
	set HERE=%cd%
	cd %CARBON_SLN%\..\
	echo %cd%
	dotnet run --project "src\Carbon.Tools\Carbon.Runner" "tools\build\runners\build.cs" %WORKING_TARGET% HOOKGEN
	cd %HERE%
) >> %OUT_LOG%

echo ** Release the dlls
(
	mkdir "%CARBON_REDIST%\server\%WORKING_TARGET_LOWER%\%PROTOCOL%\carbon\managed\hooks"
	copy "%CARBON_SLN%\..\release\.tmp\%WORKING_TARGET%\carbon\managed\hooks\Carbon.Hooks.Oxide.dll" ^
		"%CARBON_REDIST%\server\%WORKING_TARGET_LOWER%\%PROTOCOL%\carbon\managed\hooks\Carbon.Hooks.Oxide.dll"
	copy "%CARBON_SLN%\..\release\.tmp\%WORKING_TARGET%\carbon\managed\hooks\Carbon.Hooks.Community.dll" ^
		"%CARBON_REDIST%\server\%WORKING_TARGET_LOWER%\%PROTOCOL%\carbon\managed\hooks\Carbon.Hooks.Community.dll"

) 2>NUL

echo open redist.carbonmod.gg> %OUT_FTP%
echo redist@carbonmod.gg>> %OUT_FTP%
echo %FTP_PWD%>> %OUT_FTP%
echo prompt>> %OUT_FTP%
echo binary>> %OUT_FTP%
echo literal pasv>> %OUT_FTP%
echo lcd ..\..\FTP>> %OUT_FTP%
echo mkdir "hooks/server/%WORKING_TARGET_LOWER%/%PROTOCOL%">> %OUT_FTP%
echo mkdir "hooks/server/%WORKING_TARGET_LOWER%/%PROTOCOL%/carbon">> %OUT_FTP%
echo mkdir "hooks/server/%WORKING_TARGET_LOWER%/%PROTOCOL%/carbon/managed">> %OUT_FTP%
echo mkdir "hooks/server/%WORKING_TARGET_LOWER%/%PROTOCOL%/carbon/managed/hooks">> %OUT_FTP%
echo bye>> %OUT_FTP%

echo ** Build hook dll files
(
	call ftp -s:%OUT_FTP%
	del /q %OUT_FTP%
) >> %OUT_LOG%

:eof

cd %WORKDIR%