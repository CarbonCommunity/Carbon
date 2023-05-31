::
:: Copyright (c) 2022-2023 Carbon Community 
:: All rights reserved
::
@echo off

pushd %~dp0..\..\..
set UPDATE_ROOT=%CD%
popd

rem Get the target depot argument
if "%1" EQU "" (
	set UPDATE_TARGET=release
) else (
	set UPDATE_TARGET=%1
)

FOR %%O IN (windows linux) DO (
	rem Download rust binary libs
	"%UPDATE_ROOT%\Tools\DepotDownloader\DepotDownloader\bin\Release\net6.0\DepotDownloader.exe" ^
		-os %%O -validate -app 258550 -branch %UPDATE_TARGET% -filelist ^
		"%UPDATE_ROOT%\Tools\Helpers\258550_refs.txt" -dir "%UPDATE_ROOT%\Rust\%%O"

	rem Show me all you've got baby
	"%UPDATE_ROOT%\Tools\Helpers\Publicizer.exe" ^
		--input "%UPDATE_ROOT%\Rust\%%O\RustDedicated_Data\Managed\Assembly-CSharp.dll"
)

dotnet restore "%UPDATE_ROOT%\Carbon.Core" --nologo
