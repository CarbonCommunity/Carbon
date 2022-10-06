#!/usr/bin/env bash

###
### Copyright (c) 2022 Carbon Community 
### All rights reserved
###
set -e

BASE="$(cd -- "$(dirname "$0")" >/dev/null 2>&1; pwd -P)"

# Patch Rust's assemblies
chmod +x ${BASE}/carbon/tools/NStrip.exe

mono ${BASE}/carbon/tools/NStrip.exe \
	--public --include-compiler-generated --keep-resources --no-strip --overwrite \
	--unity-non-serialized ${BASE}/RustDedicated_Data/Managed/Assembly-CSharp.dll
