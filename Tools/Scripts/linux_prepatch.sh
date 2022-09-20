#!/usr/bin/env bash
set -e

SCRIPTPATH="$(cd -- "$(dirname "$0")" >/dev/null 2>&1; pwd -P)"
TARGETDLL=${SCRIPTPATH}/RustDedicated_Data/Managed/Assembly-CSharp.dll

# Patch Rust's assemblies
chmod +x ${SCRIPTPATH}/carbon/tools/NStrip
${SCRIPTPATH}/carbon/tools/NStrip -p -cg --keep-resources -n \
	--unity-non-serialized ${TARGETDLL} ${TARGETDLL}