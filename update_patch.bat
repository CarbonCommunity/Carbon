@echo off

cd Carbon.Core/Carbon.Patch
dotnet restore
msbuild /p:Configuration=Release

cd ../Carbon.Doorstep
dotnet restore
msbuild /p:Configuration=Release

cd ../Carbon
dotnet restore
msbuild /p:Configuration=Release

cd ../Carbon.Extended
dotnet restore
msbuild /p:Configuration=Release

cd ../..

"Carbon.Core/Carbon.Patch/bin/Release/Carbon.Patch.exe"
