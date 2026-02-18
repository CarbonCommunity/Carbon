@echo OFF

set ROOT=%cd%
cd ../../..

dotnet run --project Carbon.Core/Carbon.Tools/Carbon.Runner Tools/Build/runners/git_feedback.cs
cd %ROOT%