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
	set UPDATE_TARGET=public
) else (
	set UPDATE_TARGET=%1
)

"%UPDATE_ROOT%\Tools\Helpers\CodeGen.exe" ^
	--coreplugininput "%UPDATE_ROOT%\Carbon.Core\Carbon.Components\Carbon.Common\src\Carbon\Core" ^
	--corepluginoutput "%UPDATE_ROOT%\Carbon.Core\Carbon.Components\Carbon.Common\src\Generated\CorePlugin-Generated.cs"

set RUNTIME_URL=https://carbonmod.gg/assets/content/runtime-%%O.zip
set RUNTIME_FOLDER=%UPDATE_ROOT%\Tools\Runtime\%%O

FOR %%O IN (windows linux) DO (	
	if not exist %RUNTIME_FOLDER% (
		echo Downloading %%O runtime from %RUNTIME_URL%..
		mkdir %RUNTIME_FOLDER%
	
		call powershell -Command "(New-Object Net.WebClient).DownloadFile('%RUNTIME_URL%', '%RUNTIME_FOLDER%\runtime_%%O.zip')"
		call powershell -Command "Expand-Archive '%RUNTIME_FOLDER%\runtime_%%O.zip' -DestinationPath '%RUNTIME_FOLDER%'" -Force
		del "%RUNTIME_FOLDER%\runtime_%%O.zip"
	) else (
		echo Skipped %%O runtime..
	)
		
	echo Downloading %%O Rust files..

	rem Download rust binary libs
	"%UPDATE_ROOT%\Tools\DepotDownloader\DepotDownloader\bin\Release\net6.0\DepotDownloader.exe" ^
		-os %%O -validate -app 258550 -branch %UPDATE_TARGET% -filelist ^
		"%UPDATE_ROOT%\Tools\Helpers\258550_refs.txt" -dir "%UPDATE_ROOT%\Rust\%%O"

	echo Publicizing %%O Rust Assembly-CSharp..

	rem Show me all you've got baby
	"%UPDATE_ROOT%\Tools\Helpers\Publicizer.exe" ^
		--input "%UPDATE_ROOT%\Rust\%%O\RustDedicated_Data\Managed\Assembly-CSharp.dll"
		
	echo Publicizing %%O Rust Rust.Clans.Local..

	"%UPDATE_ROOT%\Tools\Helpers\Publicizer.exe" ^
		--input "%UPDATE_ROOT%\Rust\%%O\RustDedicated_Data\Managed\Rust.Clans.Local.dll"
)

dotnet restore "%UPDATE_ROOT%\Carbon.Core" --nologo
