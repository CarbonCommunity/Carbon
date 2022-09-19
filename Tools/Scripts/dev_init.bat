@echo off

rem Inits and downloads the submodules
git submodule init
git submodule update

rem Build Steam Downloading Utility
dotnet restore .\Tools\DepotDownloader
dotnet build .\Tools\DepotDownloadern --no-incremental -c Release

rem Build .NET Assembly stripper, publicizer and general utility tool
dotnet restore .\Tools\NStrip
dotnet build .\Tools\NStrip --no-incremental -c Release

rem Keeping Unity DoorStop out of the game for now due to the more complex
rem build process.

rem Download rust binary libs
.\Tools\DepotDownloader\DepotDownloader\bin\Release\net6.0\DepotDownloader.exe ^
	-app 258550 -branch public -depot 258551 -filelist .\Tools\Helpers\258550_258551_refs.txt -dir Rust .\Rust