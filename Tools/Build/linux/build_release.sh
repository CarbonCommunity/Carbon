#!/usr/bin/env bash

###
### Copyright (c) 2022 Carbon Community 
### All rights reserved
###
set -e

# Get the base path of the script
BASE="$(cd -- "$(dirname "$0")" >/dev/null 2>&1; pwd -P)"
ROOT="$(realpath "${BASE}/../../../")"

# Set the build target config
TARGET=Release

# Build the solution
dotnet restore "${ROOT}/Carbon.Core" --nologo
dotnet   clean "${ROOT}/Carbon.Core" --configuration ${TARGET} --nologo
dotnet   build "${ROOT}/Carbon.Core" --configuration ${TARGET} --no-restore --no-incremental
dotnet   build "${ROOT}/Carbon.Core" --configuration ${TARGET}Unix --no-restore --no-incremental

# Create the patch file(s)
mono "${ROOT}/Carbon.Core/Carbon.Patch/bin/${TARGET}/net48/Carbon.Patch.exe" --path "${ROOT}" --configuration ${TARGET}
