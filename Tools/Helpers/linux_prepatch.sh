#!/usr/bin/env bash
set -e

#  ______ _______ ______ ______ _______ _______ 
# |      |   _   |   __ \   __ \       |    |  |
# |   ---|       |      <   __ <   -   |       |
# |______|___|___|___|__|______/_______|__|____|
#         github.com/Carbon-Modding/Carbon.Core

BASE="$(cd -- "$(dirname "$0")" >/dev/null 2>&1; pwd -P)"
TARGETDLL=${BASE}/RustDedicated_Data/Managed/Assembly-CSharp.dll

# Patch Rust's assemblies
chmod +x ${BASE}/carbon/tools/NStrip
${BASE}/carbon/tools/NStrip -p -cg --keep-resources -n \
	--unity-non-serialized ${TARGETDLL} ${TARGETDLL}