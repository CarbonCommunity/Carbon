@echo OFF

set ROOT=%cd%
cd ../../..

dotnet run --project Carbon.Core/Carbon.Tools/Carbon.Runner Tools/Build/runners/build.cr Minimal MINIMAL edge_build -noarchive
cd %ROOT%