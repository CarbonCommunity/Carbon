::
:: Copyright (c) 2022 Carbon Community 
:: All rights reserved
::
@echo off

set BASE=%~dp0

pushd %BASE%..\..\..
set ROOT=%CD%
popd

rem Get the target depot argument
if "%1" EQU "" (
	set TARGET=public
) else (
	set TARGET=%1
)

rem Cleans the exiting files 
rem git clean -fx "%ROOT%\Rust"

FOR %%O IN (windows linux) DO (
	rem Download rust binary libs
	"%ROOT%\Tools\DepotDownloader\DepotDownloader\bin\Release\net6.0\DepotDownloader.exe" ^
		-os %%O -validate -app 258550 -branch %TARGET% -filelist ^
		"%ROOT%\Tools\Helpers\258550_258551_refs.txt" -dir "%ROOT%\Rust\%%O"

	rem Show me all you've got baby
	"%ROOT%\Tools\NStrip\NStrip\bin\Release\net452\NStrip.exe" ^
		--public --include-compiler-generated --keep-resources --no-strip --overwrite ^
		--unity-non-serialized "%ROOT%\Rust\%%O\RustDedicated_Data\Managed\Assembly-CSharp.dll" ^
		-cg --cg-exclude-events
)

dotnet restore "%ROOT%\Carbon.Core" --nologo