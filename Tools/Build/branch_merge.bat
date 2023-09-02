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

echo Merging %1 into %2

echo * Handling repository branch merge..
FOR %%P IN (Carbon.Core/Carbon.Components/Carbon.Bootstrap Carbon.Core/Carbon.Components/Carbon.Common Carbon.Core/Carbon.Components/Carbon.Compat Carbon.Core/Carbon.Components/Carbon.Modules Carbon.Core/Carbon.Components/Carbon.Preloader Carbon.Core/Carbon.Components/Carbon.SDK Carbon.Core/Carbon.Extensions/Carbon.Ext.Discord Carbon.Core/Carbon.Hooks/Carbon.Hooks.Base Carbon.Core/Carbon.Hooks/Carbon.Hooks.Oxide Carbon.Core/Carbon.Hooks/Carbon.Hooks.Community) DO (
	echo ** Merging '%%P'
	cd %BOOTSTRAP_ROOT%/%%P
    git pull origin %1 > NUL
    git push origin %2 > NUL
	echo    done.
)
echo * Finished - handling component submodules.

cd %BOOTSTRAP_ROOT%