#!/usr/bin/env bash

###
### Copyright (c) 2022-2023 Carbon Community 
### All rights reserved
###
set -e

BASE="$(cd -- "$(dirname "$0")" >/dev/null 2>&1; pwd -P)"
ROOT="$(realpath "${BASE}/../../../")"

# Get the target depot argument
TARGET=${1:-public}

for OS in windows linux; do
	# Download rust binary libs
	mono "${ROOT}/Tools/DepotDownloader_Temp/DepotDownloader.exe" \
		-os ${OS} -validate -app 258550 -branch ${TARGET} -filelist \
		"${ROOT}/Tools/Helpers/258550_refs.txt" -dir "${ROOT}/Rust/${OS}"

	# Show me all you've got baby
	mono "${ROOT}/Tools/Helpers/Publicizer.exe" \
		--input "${ROOT}/Rust/${OS}/RustDedicated_Data/Managed/Assembly-CSharp.dll"
done

dotnet restore "${ROOT}/Carbon.Core" --nologo