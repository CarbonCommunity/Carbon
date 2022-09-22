#!/usr/bin/env bash
set -e

#  ______ _______ ______ ______ _______ _______ 
# |      |   _   |   __ /   __ /       |    |  |
# |   ---|       |      <   __ <   -   |       |
# |______|___|___|___|__|______/_______|__|____|
#         github.com/Carbon-Modding/Carbon.Core

# Get the base path of the script
BASE="$(cd -- "$(dirname "$0")" >/dev/null 2>&1; pwd -P)"
ROOT="$(realpath "${BASE}/../../../")"

# Get the build target argument
TARGET=${1:-Debug}

# Build the solution
dotnet restore ${ROOT}/Carbon.Core --nologo
dotnet   clean ${ROOT}/Carbon.Core --configuration ${TARGET} --nologo
dotnet   build ${ROOT}/Carbon.Core --configuration ${TARGET} --no-restore --no-incremental
dotnet   build ${ROOT}/Carbon.Core --configuration ${TARGET}Unix --no-restore --no-incremental

# Create the patch file(s)
mono ${ROOT}/Carbon.Core/Carbon.Patch/bin/${TARGET}/net48/Carbon.Patch.exe --path ${ROOT} --configuration ${TARGET}
