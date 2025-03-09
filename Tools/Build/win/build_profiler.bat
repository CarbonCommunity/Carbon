@echo OFF

call "%~dp0\build_native.bat"

set ROOT=%cd%
cd ../../..

set VERSION=2.0.0
dotnet run --project Carbon.Core/Carbon.Tools/Carbon.Runner Tools/Build/runners/profiler.cs Debug HARMONYMOD edge_build
cd %ROOT%