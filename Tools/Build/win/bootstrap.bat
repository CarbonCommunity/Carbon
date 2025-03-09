@echo OFF

set ROOT=%cd%
cd ../../..

dotnet run --project Carbon.Core/Carbon.Tools/Carbon.Runner Tools/Build/runners/bootstrap.cs
dotnet run --project Carbon.Core/Carbon.Tools/Carbon.Runner Tools/Build/runners/update.cs
cd %ROOT%