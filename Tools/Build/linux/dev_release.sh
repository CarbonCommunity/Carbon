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

# Get the build target argument
TARGET=${1:-Debug}

# Build the solution
dotnet restore ${ROOT}/Carbon.Core --nologo
dotnet   clean ${ROOT}/Carbon.Core --configuration ${TARGET} --nologo
dotnet   build ${ROOT}/Carbon.Core --configuration ${TARGET} --no-restore --no-incremental
dotnet   build ${ROOT}/Carbon.Core --configuration ${TARGET}Unix --no-restore --no-incremental

# Update Assembly version
mono ${ROOT}/Carbon.Core/Carbon.Patch/bin/${TARGET}/net48/Carbon.Patch.exe --path ${ROOT} --versionupdate

# Rebuild the solution
dotnet   build ${ROOT}/Carbon.Core --configuration ${TARGET} --no-restore --no-incremental
dotnet   build ${ROOT}/Carbon.Core --configuration ${TARGET}Unix --no-restore --no-incremental

CERT=${ROOT}/Tools/Humanlights.SignTool/Certificate/carbon
echo ${PFXCERT}> ${CERT}.pfx_base64
base64 --decode ${CERT}.pfx_base64 ${CERT}.pfx

mono ${ROOT}/Tools/Humanlights.SignTool/Humanlights.SignTool.exe sign -folder "${ROOT}/Carbon.Core/Carbon/bin" -certificate "${CERT}.pfx" -altcertificate "${CERT}.cer" +password "${CERTPASS}" /da "sha256"

# Create the patch file(s)
mono ${ROOT}/Carbon.Core/Carbon.Patch/bin/${TARGET}/net48/Carbon.Patch.exe --path ${ROOT} --configuration ${TARGET}
