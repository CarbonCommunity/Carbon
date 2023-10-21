::
:: Copyright (c) 2022-2023 Carbon Community 
:: All rights reserved
::
@echo off

echo   ______ _______ ______ ______ _______ _______ 
echo  ^|      ^|   _   ^|   __ \   __ \       ^|    ^|  ^|
echo  ^|   ---^|       ^|      ^<   __ ^<   -   ^|       ^|
echo  ^|______^|___^|___^|___^|__^|______/_______^|__^|____^|
echo                           discord.gg/carbonmod
echo.

pushd %~dp0..\..
set BOOTSTRAP_ROOT=%CD%
popd

set FROM_BRANCH=%1
set TO_BRANCH=%2

if "%FROM_BRANCH%" EQU "" (
	set /p FROM_BRANCH=Enter branch to merge from: 
)

if "%TO_BRANCH%" EQU "" (
	set /p TO_BRANCH=Enter branch to merge into: 
)

echo Merging %FROM_BRANCH% into %TO_BRANCH%..

FOR %%P IN (Carbon.Core/Carbon.Components/Carbon.Bootstrap Carbon.Core/Carbon.Components/Carbon.Common Carbon.Core/Carbon.Components/Carbon.Common.Client Carbon.Core/Carbon.Components/Carbon.Compat Carbon.Core/Carbon.Components/Carbon.Modules Carbon.Core/Carbon.Components/Carbon.Preloader Carbon.Core/Carbon.Components/Carbon.SDK Carbon.Core/Carbon.Components/Carbon.Compiler Carbon.Core/Carbon.Extensions/Carbon.Ext.Discord Carbon.Core/Carbon.Hooks/Carbon.Hooks.Base Carbon.Core/Carbon.Hooks/Carbon.Hooks.Oxide Carbon.Core/Carbon.Hooks/Carbon.Hooks.Community) DO (
	echo ** Merging '%%P'
	cd %BOOTSTRAP_ROOT%/%%P
	git checkout %TO_BRANCH% > NUL
	git merge origin/%FROM_BRANCH% -m "Merging %FROM_BRANCH% into %TO_BRANCH%" --no-ff > NUL
	git push --set-upstream origin %TO_BRANCH% > NUL
	echo    done.
)
echo * Finished - branch merge.

cd %BOOTSTRAP_ROOT%