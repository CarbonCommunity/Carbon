::
:: Copyright (c) 2022 Carbon Community 
:: All rights reserved
::
@echo off

set BASE=%~dp0

pushd %BASE%\..\..\RustDedicated_Data\Managed
set WORKING=%CD%
popd

set FILE=Assembly-CSharp

copy /Y "%WORKING%\%FILE%.dll" "%WORKING%\%FILE%-bak.dll"
copy /Y "%WORKING%\%FILE%-pub.dll" "%WORKING%\%FILE%.dll"