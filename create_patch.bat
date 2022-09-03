@echo off

cd Carbon.Core/Carbon.Patch
dotnet restore
msbuild /p:Configuration=Release

cd ../Carbon
dotnet restore
msbuild /p:Configuration=Release

cd ../..
echo %cd%

"Carbon.Core/Carbon.Patch/bin/Release/Carbon.Patch.exe"
