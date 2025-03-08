@echo OFF

set ROOT=%cd%
cd ../../..

dotnet run --project Carbon.Core/Carbon.Tools/Carbon.Runner Tools/Build/runners/build.cr Debug EDGE edge_build
cd %ROOT%