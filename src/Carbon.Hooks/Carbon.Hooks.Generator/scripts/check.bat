@echo OFF

:: Validates the local OPJ (.rust\Rust.opj) against the Rust managed assemblies without
:: touching it. Exit code 0 means clean, 2 means at least one hook needs human attention.
::
:: Usage: check.bat [managed folder] [old managed folder]
::   MANAGED     - overrides the managed folder (default: <repo>\rust\windows\RustDedicated_Data\Managed)
::   OLD_MANAGED - previous game build's managed folder, used to re-anchor drifted injection indexes
::   OPJ         - overrides the OPJ being checked (default: <generator>\.rust\Rust.opj)
::   CHECK_OUTPUT- overrides the JSON report path (default: <generator>\.rust\Rust.check.json)

setlocal

pushd %~dp0..
set WORKDIR=%CD%
popd

pushd %~dp0..\..\..\..
set CARBON_ROOT=%CD%
popd

set TOOLS=%WORKDIR%\src

if "%OPJ%" EQU "" set OPJ=%WORKDIR%\.rust\Rust.opj
if "%CHECK_OUTPUT%" EQU "" set CHECK_OUTPUT=%WORKDIR%\.rust\Rust.check.json
if "%MANAGED%" EQU "" set MANAGED=%CARBON_ROOT%\rust\windows\RustDedicated_Data\Managed

if NOT "%~1" EQU "" set MANAGED=%~1
if NOT "%~2" EQU "" set OLD_MANAGED=%~2

if NOT EXIST "%OPJ%" (
	echo ** OPJ file not found: '%OPJ%'
	echo ** Run build.bat with an OPJ endpoint, or drop one at that path.
	exit /b 1
)

if NOT EXIST "%MANAGED%" (
	echo ** Managed folder not found: '%MANAGED%'
	echo ** Run setup.bat at the repository root, or pass the folder as the first argument.
	exit /b 1
)

set OLD_MANAGED_ARG=
if NOT "%OLD_MANAGED%" EQU "" set OLD_MANAGED_ARG=--old-managed "%OLD_MANAGED%"

echo ---------------------------------------------------------------------------
echo             OPJ: %OPJ%
echo  Managed folder: %MANAGED%
if NOT "%OLD_MANAGED%" EQU "" echo      Old managed: %OLD_MANAGED%
echo          Report: %CHECK_OUTPUT%
echo ---------------------------------------------------------------------------

echo ** Checking hooks
dotnet run --project "%TOOLS%" -- ^
	--check ^
	--input "%OPJ%" ^
	--managed "%MANAGED%" ^
	--check-output "%CHECK_OUTPUT%" %OLD_MANAGED_ARG%

set RESULT=%ERRORLEVEL%

if "%RESULT%" EQU "0" (
	echo ** Check passed
) else if "%RESULT%" EQU "2" (
	echo ** Check found hooks that need human input - see '%CHECK_OUTPUT%'
	echo ** Run fix.bat to apply the automatic fixes
) else (
	echo ** Check failed with exit code %RESULT%
)

endlocal & exit /b %RESULT%
