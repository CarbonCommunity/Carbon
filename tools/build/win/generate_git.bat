@echo OFF

set ROOT=%cd%
cd ../../..

dotnet run --project src/Carbon.Tools/Carbon.Runner Tools/Build/runners/git_feedback.cs
cd %ROOT%