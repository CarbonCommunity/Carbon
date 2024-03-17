::
:: Copyright (c) 2022-2023 Carbon Community 
:: All rights reserved
::
@echo off

set ROOT="%cd%/Tools/Build/win"

cd %ROOT%
bootstrap.bat

cd %ROOT%
build.bat