@echo OFF

set ROOT=%cd%
cd ../../..

dotnet run --project src/Carbon.Tools/Carbon.Runner tools/build/runners/build.cs Release EDGE edge_build -norestore
cd %ROOT%