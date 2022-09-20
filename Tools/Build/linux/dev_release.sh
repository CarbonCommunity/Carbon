#!/usr/bin/env bash
set -e

#  ______ _______ ______ ______ _______ _______ 
# |      |   _   |   __ /   __ /       |    |  |
# |   ---|       |      <   __ <   -   |       |
# |______|___|___|___|__|______/_______|__|____|
#         github.com/Carbon-Modding/Carbon.Core

BASE="$(cd -- "$(dirname "$0")" >/dev/null 2>&1; pwd -P)"
ROOT="$(realpath "${BASE}/../../../")"

# Build the solution
#dotnet clean ${ROOT}/Carbon.Core -t:Cleanup --configuration Release
#dotnet build ${ROOT}/Carbon.Core --configuration Release --no-incremental
#dotnet build ${ROOT}/Carbon.Core --configuration ReleaseUnix --no-incremental

"${ROOT}/Carbon.Core/Carbon.Patch/bin/Release/net48/Carbon.Patch.exe" --path ${ROOT}
