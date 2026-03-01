@echo OFF

set ROOT=%cd%
cd ../../..

dotnet run --project src/Carbon.Tools/Carbon.Runner Tools/Build/runners/update.cs aux01-staging
cd %ROOT%