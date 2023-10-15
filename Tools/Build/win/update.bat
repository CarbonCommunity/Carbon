::
:: Copyright (c) 2022-2023 Carbon Community 
:: All rights reserved
::
@echo off

pushd %~dp0..\..\..
set UPDATE_ROOT=%CD%
set RUNTIME_URL=https://carbonmod.gg/assets/content/runtime.zip
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

echo Downloading Runtime from '%RUNTIME_URL%'..

call powershell -Command "(New-Object Net.WebClient).DownloadFile('%RUNTIME_URL%', '%UPDATE_ROOT%\Tools\Runtime\runtime.zip')" || exit /b
call powershell -Command "Expand-Archive '%UPDATE_ROOT%\Tools\Runtime\runtime.zip' -DestinationPath '%UPDATE_ROOT%\Tools\Runtime'" -Force
del "%UPDATE_ROOT%\Tools\Runtime\runtime.zip"

echo Downloading depots using '%UPDATE_TARGET%' Steam branch..

FOR %%O IN (windows linux) DO (
	rem Download rust binary libs
	"%UPDATE_ROOT%\Tools\DepotDownloader\DepotDownloader\bin\Release\net6.0\DepotDownloader.exe" ^
		-os %%O -validate -app 258550 -branch %UPDATE_TARGET% -filelist ^
		"%UPDATE_ROOT%\Tools\Helpers\258550_refs.txt" -dir "%UPDATE_ROOT%\Rust\%%O"

	rem Show me all you've got baby
	"%UPDATE_ROOT%\Tools\Helpers\Publicizer.exe" ^
		--input "%UPDATE_ROOT%\Rust\%%O\RustDedicated_Data\Managed\Assembly-CSharp.dll"
		
	"%UPDATE_ROOT%\Tools\Helpers\Publicizer.exe" ^
		--input "%UPDATE_ROOT%\Rust\%%O\RustDedicated_Data\Managed\Rust.Clans.Local.dll"
)

dotnet restore "%UPDATE_ROOT%\Carbon.Core" --nologo
