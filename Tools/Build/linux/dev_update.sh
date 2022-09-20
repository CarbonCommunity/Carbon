#!/usr/bin/env bash
set -e

#  ______ _______ ______ ______ _______ _______ 
# |      |   _   |   __ /   __ /       |    |  |
# |   ---|       |      <   __ <   -   |       |
# |______|___|___|___|__|______/_______|__|____|
#         github.com/Carbon-Modding/Carbon.Core

BASE="$(cd -- "$(dirname "$0")" >/dev/null 2>&1; pwd -P)"
ROOT="$(realpath "${BASE}/../../../")"

# Download rust binary libs
${ROOT}/Tools/DepotDownloader/DepotDownloader/bin/Release/net6.0/DepotDownloader \
	-app 258550 -branch public -depot 258551 -filelist \
	${ROOT}/Tools/Helpers/258550_258551_refs.txt -dir ${ROOT}/Rust

# Show me all you've got baby
mono ${ROOT}/Tools/NStrip/NStrip/bin/Release/net452/NStrip.exe \
	-p -cg --keep-resources -n --unity-non-serialized \
	${ROOT}/Rust/RustDedicated_Data/Managed/Assembly-CSharp.dll \
	${ROOT}/Rust/RustDedicated_Data/Managed/Assembly-CSharp.dll
