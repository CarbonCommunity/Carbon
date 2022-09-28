#!/usr/bin/env bash
set -e

cat <<EOF
  ______ _______ ______ ______ _______ _______ 
 |      |   _   |   __ \   __ \       |    |  |
 |   ---|       |      <   __ <   -   |       |
 |______|___|___|___|__|______/_______|__|____|
                         discord.gg/eXPcNKK4yd

EOF

BASE="$(cd -- "$(dirname "$0")" >/dev/null 2>&1; pwd -P)"
ROOT="$(realpath "${BASE}/../../../")"

# Get the target depot argument
TARGET=${1:-public}

# Cleans the exiting files
git clean -fx "${ROOT}/Rust"

# Download rust binary libs
"${ROOT}/Tools/DepotDownloader/DepotDownloader/bin/Release/net6.0/DepotDownloader" \
	-validate -app 258550 -depot 258551 -branch ${TARGET} -filelist \
	"${ROOT}/Tools/Helpers/258550_258551_refs.txt" -dir "${ROOT}/Rust"

# Show me all you've got baby
mono "${ROOT}/Tools/NStrip/NStrip/bin/Release/net452/NStrip.exe" \
	--public --include-compiler-generated --keep-resources --no-strip --overwrite \
	--unity-non-serialized "${ROOT}/Rust/RustDedicated_Data/Managed/Assembly-CSharp.dll"

dotnet restore "${ROOT}/Carbon.Core" --nologo