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

# Build the solution
dotnet restore "${ROOT}/Carbon.Core" -v:m --nologo
dotnet   clean "${ROOT}/Carbon.Core" -v:m --configuration ${TARGET} --nologo
dotnet   build "${ROOT}/Carbon.Core" -v:m --configuration ${TARGET} --no-restore --no-incremental

# Copy doorstop helper files (windows)
cp "${ROOT}/Tools/Helpers/doorstop_config.ini" "${ROOT}/Release/.tmp/${TARGET}"
cp "${ROOT}/Tools/UnityDoorstop/windows/x64/doorstop.dll" "${ROOT}/Release/.tmp/${TARGET}/winhttp.dll"

if [[ "${TARGET}" == *"Unix"* ]]; then
	# Copy doorstop helper files (unix)
	cp "${ROOT}/Tools/Helpers/environment.sh" "${ROOT}/Release/.tmp/${TARGET}/carbon/tools"
	cp "${ROOT}/Tools/Helpers/publicizer.sh" "${ROOT}/Release/.tmp/${TARGET}/carbon/tools"
fi

rem Create the standalone files
cp /y "${ROOT}/Release/.tmp/${TARGET}/HarmonyMods/Carbon.Loader.dll" "${ROOT}/Release"
cp /y "${ROOT}/Release/.tmp/${TARGET}/carbon/managed/Carbon.dll" "${ROOT}/Release"
cp /y "${ROOT}/Release/.tmp/${TARGET}/carbon/managed/Carbon.Doorstop.dll" "${ROOT}/Release"

# Create the zip archive release files
cd "${ROOT}/Release/.tmp/${TARGET}" && zip -r "${ROOT}/Release/Carbon.${TARGET}.zip" .