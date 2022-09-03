@echo off

cd Carbon.Core/Carbon.Patch
dotnet restore
msbuild /p:Configuration=Release

cd ../Carbon
dotnet restore
msbuild /p:Configuration=Release

echo %cd%

cd ../..
echo %cd%

"Carbon.Core/Carbon.Patch/bin/Release/Carbon.Patch.exe"