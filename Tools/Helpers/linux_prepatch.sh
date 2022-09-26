#!/usr/bin/env bash
set -e

#  ______ _______ ______ ______ _______ _______ 
# |      |   _   |   __ \   __ \       |    |  |
# |   ---|       |      <   __ <   -   |       |
# |______|___|___|___|__|______/_______|__|____|
#         github.com/Carbon-Modding/Carbon.Core

BASE="$(cd -- "$(dirname "$0")" >/dev/null 2>&1; pwd -P)"

# Patch Rust's assemblies
chmod +x ${BASE}/carbon/tools/NStrip.exe

mono ${BASE}/carbon/tools/NStrip.exe \
	--public --include-compiler-generated --keep-resources --no-strip --overwrite \
	--unity-non-serialized ${BASE}/RustDedicated_Data/Managed/Assembly-CSharp.dll
