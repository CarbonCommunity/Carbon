@echo OFF

set ROOT=%cd%
cd ../../..

dotnet run --project Carbon.Core/Carbon.Tools/Carbon.Runner Tools/Build/runners/patcher_setup.cs release "xx"
cd %ROOT%