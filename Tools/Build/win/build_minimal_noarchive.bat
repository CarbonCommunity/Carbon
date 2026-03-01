@echo OFF

set ROOT=%cd%
cd ../../..

dotnet run --project src/Carbon.Tools/Carbon.Runner Tools/Build/runners/build.cs Minimal MINIMAL edge_build -noarchive -norestore
cd %ROOT%