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

pushd %~dp0..\..\..
set BOOTSTRAP_ROOT=%CD%
popd

rem Inits and downloads the submodules
git submodule init
git submodule update

FOR %%O IN (DepotDownloader) DO (
	dotnet restore "%BOOTSTRAP_ROOT%\Tools\%%O" --verbosity quiet --nologo --force 
	dotnet clean   "%BOOTSTRAP_ROOT%\Tools\%%O" --verbosity quiet --configuration Release --nologo
	dotnet build   "%BOOTSTRAP_ROOT%\Tools\%%O" --verbosity quiet --configuration Release --no-restore --no-incremental
)

rem Download rust binary libs
call "%~dp0\update.bat" public

rem Don't track changes to this file
git update-index --assume-unchanged "%BOOTSTRAP_ROOT%\Tools\Helpers\doorstop_config.ini"
