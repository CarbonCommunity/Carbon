@echo off

set SCRIPTPATH=%~dp0..\..

rem Download rust binary libs
%SCRIPTPATH%\Tools\DepotDownloader\DepotDownloader\bin\Release\net6.0\DepotDownloader.exe ^
	-app 258550 -branch public -depot 258551 -filelist %SCRIPTPATH%\Tools\Helpers\258550_258551_refs.txt -dir %SCRIPTPATH%\Rust

rem Show me all you've got baby
%SCRIPTPATH%\Tools\NStrip\NStrip\bin\Release\net452\NStrip.exe ^
	-p -cg --keep-resources -n --unity-non-serialized %SCRIPTPATH%\Rust\RustDedicated_Data\Managed\Assembly-CSharp.dll %SCRIPTPATH%\Rust\RustDedicated_Data\Managed\Assembly-CSharp.dll
