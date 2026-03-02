@echo OFF

call "%~dp0\build_native.bat"

set ROOT=%cd%
cd ../../..

set VERSION=2.0.0
dotnet run --project src/Carbon.Tools/Carbon.Runner tools/build/runners/profiler.cs Debug HARMONYMOD edge_build -noarchive
dotnet run --project src/Carbon.Tools/Carbon.Runner tools/build/runners/profiler.cs DebugUnix HARMONYMOD edge_build -noarchive
cd %ROOT%