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
TARGET=${1:-Debug}

# Cleanup the release folder
rm -rf "${ROOT}/Release/.tmp/${TARGET}" "${ROOT}/Release/Carbon.${TARGET}.zip"
rm -rf "${ROOT}/Release/.tmp/${TARGET}Unix" "${ROOT}/Release/Carbon.${TARGET}Unix.zip"

# Build the solution
dotnet restore "${ROOT}/Carbon.Core" --nologo
dotnet   clean "${ROOT}/Carbon.Core" --configuration ${TARGET} --nologo
dotnet   clean "${ROOT}/Carbon.Core" --configuration ${TARGET}Unix --nologo
dotnet   build "${ROOT}/Carbon.Core" --configuration ${TARGET} --no-restore --no-incremental
dotnet   build "${ROOT}/Carbon.Core" --configuration ${TARGET}Unix --no-restore --no-incremental

# Copy doorstop helper files (windows)
cp "${ROOT}/Tools/Helpers/doorstop_config.ini" "${ROOT}/Release/.tmp/${TARGET}"
cp "${ROOT}/Tools/UnityDoorstop/windows/x64/doorstop.dll" "${ROOT}/Release/.tmp/${TARGET}/winhttp.dll"

# Copy doorstop helper files (unix)
cp "${ROOT}/Tools/Helpers/publicizer.sh" "${ROOT}/Release/.tmp/${TARGET}Unix/carbon/tools"

# Create the zip archive release files
cd "${ROOT}/Release/.tmp/${TARGET}" && zip -r "${ROOT}/Release/Carbon.${TARGET}.zip" .
cd "${ROOT}/Release/.tmp/${TARGET}Unix" && zip -r "${ROOT}/Release/Carbon.${TARGET}Unix.zip" .