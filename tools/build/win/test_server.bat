@echo off
setlocal EnableDelayedExpansion

:: Builds Carbon (Debug), deploys it into a local Rust server under .tmp\servers\<branch>
:: and starts it on QAIsland. The server is downloaded with DepotDownloader on first run.
::
:: Usage: test_server.bat [-nobuild] [-hooks] [-update] [-branch <steam branch>] [-level <server.level>]
::   -nobuild  skip the Carbon build, deploy what's already in release\.tmp\Debug
::   -hooks    regenerate Carbon.Hooks.Oxide from the local OPJ and build it (HOOKGEN)
::             instead of downloading the published protocol hooks
::   -update   re-download / validate the Rust server before starting
::   -branch   Steam branch (default: derived from the git branch, rust_beta/<x> -> <x>, else release)
::   -level    server.level (default: QAIsland)

for %%I in ("%~dp0..\..\..") do set "ROOT=%%~fI"

set "BUILD=1"
set "HOOKS=0"
set "UPDATE=0"
set "BRANCH="
set "LEVEL=QAIsland"

:args
if "%~1"=="" goto args_done
if /i "%~1"=="-nobuild" set "BUILD=0"
if /i "%~1"=="-hooks" set "HOOKS=1"
if /i "%~1"=="-update" set "UPDATE=1"
if /i "%~1"=="-branch" set "BRANCH=%~2" & shift
if /i "%~1"=="-level" set "LEVEL=%~2" & shift
shift
goto args
:args_done

if not defined BRANCH (
	set "BRANCH=staging"
	for /f "delims=" %%B in ('git -C "%ROOT%" rev-parse --abbrev-ref HEAD 2^>NUL') do set "GITBRANCH=%%B"
	if /i "!GITBRANCH:~0,10!"=="rust_beta/" set "BRANCH=!GITBRANCH:~10!"
)

set "SERVER=%ROOT%\.tmp\servers\%BRANCH%"
set "RELEASE=%ROOT%\release\.tmp\Debug"

echo ---------------------------------------------------------------------------
echo   Steam branch: %BRANCH%
echo         Server: %SERVER%
echo          Level: %LEVEL%
echo ---------------------------------------------------------------------------

:: Rust server
if not exist "%SERVER%\RustDedicated.exe" set "UPDATE=1"
if "%UPDATE%"=="1" (
	echo ** Downloading Rust server [%BRANCH%]
	dotnet build "%ROOT%\tools\depot\DepotDownloader" -v q -nologo || exit /b 1
	dotnet run --no-build --project "%ROOT%\tools\depot\DepotDownloader" -- ^
		-app 258550 -os windows -branch %BRANCH% -validate -dir "%SERVER%" || exit /b 1
)

:: Carbon
if "%BUILD%"=="1" (
	if "%HOOKS%"=="1" (
		echo ** Generating Oxide hooks from the local OPJ
		set "GEN=%ROOT%\src\Carbon.Hooks\Carbon.Hooks.Generator"
		set "OXIDE_SRC=%ROOT%\src\Carbon.Hooks\Carbon.Hooks.Oxide\src"
		del /q "!OXIDE_SRC!\*.cs" 2>NUL
		dotnet run --project "!GEN!\src" -- --input "!GEN!\.rust\Rust.opj" ^
			--managed "%ROOT%\rust\windows\RustDedicated_Data\Managed" --output "!OXIDE_SRC!" || exit /b 1

		echo ** Building Carbon [Debug, HOOKGEN]
		call "%~dp0build.bat" Debug HOOKGEN edge_build -noarchive || exit /b 1
	) else (
		echo ** Building Carbon [Debug]
		call "%~dp0build_debug_noarchive.bat" || exit /b 1
	)
)

if not exist "%RELEASE%\carbon\managed\Carbon.dll" (
	echo ** No Carbon build at '%RELEASE%', run without -nobuild
	exit /b 1
)

echo ** Deploying Carbon to the server
robocopy "%RELEASE%" "%SERVER%" /E /NFL /NDL /NJH /NJS /NP >NUL
if %ERRORLEVEL% GEQ 8 (
	echo ** Deploy failed [robocopy %ERRORLEVEL%]
	exit /b 1
)

:: Local builds are unversioned; keep the preloader / runtime from replacing them with edge
echo ** Disabling self-updating in carbon\config.json
powershell -NoProfile -ExecutionPolicy Bypass -Command "$p = Join-Path $env:SERVER 'carbon\config.json'; if (Test-Path $p) { $c = Get-Content $p -Raw | ConvertFrom-Json } else { $c = [pscustomobject]@{} }; if (-not $c.SelfUpdating) { $c | Add-Member -NotePropertyName SelfUpdating -NotePropertyValue ([pscustomobject]@{}) -Force }; $c.SelfUpdating | Add-Member -NotePropertyName Enabled -NotePropertyValue $false -Force; $c.SelfUpdating | Add-Member -NotePropertyName HookUpdates -NotePropertyValue $false -Force; $c | ConvertTo-Json -Depth 32 | Set-Content $p -Encoding UTF8" || exit /b 1

echo ** Starting the server
pushd "%SERVER%"
"%SERVER%\RustDedicated.exe" -nographics -batchmode -logs -silent-crashes ^
	-server.hostname "Carbon Test Server" ^
	-server.identity "carbon" ^
	-server.level "%LEVEL%" ^
	-server.port 28015 ^
	-server.queryport 28016 ^
	-rcon.port 28017 ^
	-rcon.password "carbondev" ^
	-rcon.web true ^
	-server.maxplayers 10 ^
	-server.saveinterval 400 ^
	-chat.serverlog 1 ^
	+baseboat.generate_paths 0 ^
	-logfile "carbon_log.txt"
set "EXIT_CODE=%ERRORLEVEL%"
popd

endlocal & exit /b %EXIT_CODE%
