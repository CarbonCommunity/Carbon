@echo OFF

cd ../../..

dotnet run --project Carbon.Core/Carbon.Tools/Carbon.Runner Tools/Build/runners/bootstrap.cr
dotnet run --project Carbon.Core/Carbon.Tools/Carbon.Runner Tools/Build/runners/update.cr