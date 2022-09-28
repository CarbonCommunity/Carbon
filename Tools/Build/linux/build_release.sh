#!/usr/bin/env bash
set -e

cat <<EOF
  ______ _______ ______ ______ _______ _______ 
 |      |   _   |   __ \   __ \       |    |  |
 |   ---|       |      <   __ <   -   |       |
 |______|___|___|___|__|______/_______|__|____|
                         discord.gg/eXPcNKK4yd

EOF

# Get the base path of the script
BASE="$(cd -- "$(dirname "$0")" >/dev/null 2>&1; pwd -P)"
ROOT="$(realpath "${BASE}/../../../")"

# Set the build target config
TARGET=Release

# Build the solution + generate identifier
dotnet restore "${ROOT}/Carbon.Core" --nologo
dotnet   clean "${ROOT}/Carbon.Core" --configuration ${TARGET} --nologo
dotnet   build "${ROOT}/Carbon.Core" --configuration ${TARGET} --no-restore --no-incremental
dotnet   build "${ROOT}/Carbon.Core" --configuration ${TARGET}Unix --no-restore --no-incremental

# Build the solution for actual release
dotnet restore "${ROOT}/Carbon.Core" --nologo
dotnet   clean "${ROOT}/Carbon.Core" --configuration ${TARGET} --nologo
dotnet   build "${ROOT}/Carbon.Core" --configuration ${TARGET} --no-restore --no-incremental
dotnet   build "${ROOT}/Carbon.Core" --configuration ${TARGET}Unix --no-restore --no-incremental

# Create the patch file(s)
mono "${ROOT}/Carbon.Core/Carbon.Patch/bin/${TARGET}/net48/Carbon.Patch.exe" --path "${ROOT}" --configuration ${TARGET}
