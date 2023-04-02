::
:: Copyright (c) 2022-2023 Carbon Community 
:: All rights reserved
::
@echo off

echo   ______ _______ ______ ______ _______ _______ 
echo  ^|      ^|   _   ^|   __ \   __ \       ^|    ^|  ^|
echo  ^|   ---^|       ^|      ^<   __ ^<   -   ^|       ^|
echo  ^|______^|___^|___^|___^|__^|______/_______^|__^|____^|
echo                         discord.gg/eXPcNKK4yd
echo.

set BASE=%~dp0

pushd %BASE%..\..\..
set ROOT=%CD%
popd

rem Inits and downloads the submodules
git submodule init
git submodule update

:: rem Changes the assembly name for HamonyLib [requires powershell]
:: set HARMONYDIR=%ROOT%\Tools\HarmonyLib\Harmony
:: powershell -Command "(Get-Content -path '%HARMONYDIR%\Harmony.csproj') -replace '0Harmony', '1Harmony' | Out-File '%HARMONYDIR%\Harmony.csproj'"

FOR %%O IN (DepotDownloader NStrip) DO (
	dotnet restore "%ROOT%\Tools\%%O" --verbosity quiet --nologo --force 
	dotnet clean   "%ROOT%\Tools\%%O" --verbosity quiet --configuration Release --nologo
	dotnet build   "%ROOT%\Tools\%%O" --verbosity quiet --configuration Release --no-restore --no-incremental
)

rem Download rust binary libs
call "%BASE%\update.bat" public

rem Don't track changes to this file
git update-index --assume-unchanged "%ROOT%\Tools\Helpers\doorstop_config.ini"