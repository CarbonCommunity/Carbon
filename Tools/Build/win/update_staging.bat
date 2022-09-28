@echo off

echo   ______ _______ ______ ______ _______ _______ 
echo  ^|      ^|   _   ^|   __ \   __ \       ^|    ^|  ^|
echo  ^|   ---^|       ^|      ^<   __ ^<   -   ^|       ^|
echo  ^|______^|___^|___^|___^|__^|______/_______^|__^|____^|
echo                         discord.gg/eXPcNKK4yd
echo.

set BASE=%~dp0

pushd %BASE%..\..\..
set ROOT=%CD%
popd

rem Cleans the exiting files
git clean -fx "%ROOT%\Rust\RustDedicated_Data"

rem Download rust binary libs
"%ROOT%\Tools\DepotDownloader\DepotDownloader\bin\Release\net6.0\DepotDownloader.exe" ^
	-app 258550 -branch staging -depot 258551 -filelist ^
	"%ROOT%\Tools\Helpers\258550_258551_refs.txt" -dir "%ROOT%\Rust"

rem Show me all you've got baby
"%ROOT%\Tools\NStrip\NStrip\bin\Release\net452\NStrip.exe" ^
	--public --include-compiler-generated --keep-resources --no-strip --overwrite ^
	--unity-non-serialized "%ROOT%\Rust\RustDedicated_Data\Managed\Assembly-CSharp.dll"

dotnet restore "%ROOT%\Carbon.Core" --nologo