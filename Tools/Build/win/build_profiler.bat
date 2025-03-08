@echo OFF

cd ../../..

set VERSION=2.0.0
dotnet run --project Carbon.Core/Carbon.Tools/Carbon.Runner Tools/Build/runners/profiler.cr Debug HARMONYMOD