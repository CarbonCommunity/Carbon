::
:: Copyright (c) 2022-2023 Carbon Community 
:: All rights reserved
::
@echo off

set SUPERROOT="%cd%\Tools\Build\win"

cd %SUPERROOT%
call bootstrap.bat

cd %SUPERROOT%
call build_debug_noarchive.bat