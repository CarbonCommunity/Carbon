#!/usr/bin/env bash

###
### Copyright (c) 2022 Carbon Community 
### All rights reserved
###
set -e

BASE="$(cd -- "$(dirname "$0")" >/dev/null 2>&1; pwd -P)"
ROOT="$(realpath "${BASE}/../../../")"

# Get the target depot argument
TARGET=${1:-public}

# Cleans the exiting files
git clean -fx "${ROOT}/Rust"

for OS in windows linux; do
	# Download rust binary libs
	"${ROOT}/Tools/DepotDownloader/DepotDownloader/bin/Release/net6.0/DepotDownloader" \
		-os ${OS} -validate -app 258550 -branch ${TARGET} -filelist \
		"${ROOT}/Tools/Helpers/258550_258551_refs.txt" -dir "${ROOT}/Rust/${OS}"

	# Show me all you've got baby
	mono "${ROOT}/Tools/NStrip/NStrip/bin/Release/net452/NStrip.exe" \
		--public --include-compiler-generated --keep-resources --no-strip --overwrite \
		--unity-non-serialized "${ROOT}/Rust/${OS}/RustDedicated_Data/Managed/Assembly-CSharp.dll"
done

dotnet restore "${ROOT}/Carbon.Core" --nologo