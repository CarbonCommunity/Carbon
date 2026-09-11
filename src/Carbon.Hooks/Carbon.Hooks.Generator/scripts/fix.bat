@echo OFF

:: Runs the OPJ validation on the local OPJ (.rust\Rust.opj) and applies every automatic fix,
:: writing the result to .rust\Rust.fixed.opj. Exit code 0 means everything was fixed, 2 means
:: leftovers need human attention.
::
:: Usage: fix.bat [managed folder] [old managed folder]
::   MANAGED     - overrides the managed folder (default: <repo>\rust\windows\RustDedicated_Data\Managed)
::   OLD_MANAGED - previous game build's managed folder, used to re-anchor drifted injection indexes
::   OPJ         - overrides the OPJ being fixed (default: <generator>\.rust\Rust.opj)
::   FIX_OUTPUT  - overrides the fixed OPJ path (default: <generator>\.rust\Rust.fixed.opj)
::   CHECK_OUTPUT- overrides the JSON report path (default: <generator>\.rust\Rust.fix.json)
::   INPLACE     - set to 1 to back up the input as '<OPJ>.bak' and replace it with the fixed OPJ

setlocal

pushd %~dp0..
set WORKDIR=%CD%
popd

pushd %~dp0..\..\..\..
set CARBON_ROOT=%CD%
popd

set TOOLS=%WORKDIR%\src

if "%OPJ%" EQU "" set OPJ=%WORKDIR%\.rust\Rust.opj
if "%FIX_OUTPUT%" EQU "" set FIX_OUTPUT=%WORKDIR%\.rust\Rust.fixed.opj
if "%CHECK_OUTPUT%" EQU "" set CHECK_OUTPUT=%WORKDIR%\.rust\Rust.fix.json
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
echo      Fixed  OPJ: %FIX_OUTPUT%
echo          Report: %CHECK_OUTPUT%
echo ---------------------------------------------------------------------------

echo ** Fixing hooks
dotnet run --project "%TOOLS%" -- ^
	--fix ^
	--input "%OPJ%" ^
	--managed "%MANAGED%" ^
	--fix-output "%FIX_OUTPUT%" ^
	--check-output "%CHECK_OUTPUT%" %OLD_MANAGED_ARG%

set RESULT=%ERRORLEVEL%

if "%RESULT%" NEQ "0" if "%RESULT%" NEQ "2" (
	echo ** Fix failed with exit code %RESULT%
	endlocal & exit /b %RESULT%
)

if NOT EXIST "%FIX_OUTPUT%" (
	echo ** No fixed OPJ was written
	endlocal & exit /b %RESULT%
)

if "%INPLACE%" EQU "1" (
	echo ** Backing up '%OPJ%' to '%OPJ%.bak'
	copy /y "%OPJ%" "%OPJ%.bak" >NUL || exit /b 1
	move /y "%FIX_OUTPUT%" "%OPJ%" >NUL || exit /b 1
	echo ** '%OPJ%' updated in place
) else (
	echo ** Fixed OPJ written to '%FIX_OUTPUT%'
	echo ** Re-run with INPLACE=1 to replace '%OPJ%' with it
)

if "%RESULT%" EQU "2" (
	echo ** Some hooks still need human input - see '%CHECK_OUTPUT%'
) else (
	echo ** Everything is clean
)

endlocal & exit /b %RESULT%
