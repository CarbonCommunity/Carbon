@echo OFF

set ROOT=%cd%
cd ../../..

dotnet run --project Carbon.Core/Carbon.Tools/Carbon.Runner Tools/Build/runners/build.cs Debug HOOKGEN edge_build -noarchive
cd %ROOT%