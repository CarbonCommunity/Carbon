@echo OFF

set ROOT=%cd%
cd ../../..

dotnet run --project src/Carbon.Tools/Carbon.Runner tools/build/runners/build.cs Minimal MINIMAL edge_build -norestore
cd %ROOT%