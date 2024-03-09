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

mono "${ROOT}/Tools/Helpers/CodeGen.exe" \
	--plugininput "${ROOT}/Carbon.Core/Carbon.Components/Carbon.Common/src/Carbon/Core" \
	--pluginoutput "${ROOT}/Carbon.Core/Carbon.Components/Carbon.Common/src/Generated/CorePlugin-Generated.cs"

for MODULE in GatherManagerModule ModerationToolsModule OptimisationsModule StackManagerModule VanishModule WhitelistModule; do
	mono "${ROOT}/Tools/Helpers/CodeGen.exe" \
		--plugininput "${ROOT}/Carbon.Core/Carbon.Components/Carbon.Modules/src/${MODULE}" \
		--pluginoutput "${ROOT}/Carbon.Core/Carbon.Components/Carbon.Modules/src/${MODULE}/${MODULE}-Generated.cs" \
		--pluginname "${MODULE}" \
		--pluginnamespace "Carbon.Modules"

done

for OS in windows linux; do
	# Download rust binary libs
	"${ROOT}/Tools/DepotDownloader/DepotDownloader/bin/Release/net6.0/DepotDownloader" \
		-os ${OS} -validate -app 258550 -branch ${TARGET} -filelist \
		"${ROOT}/Tools/Helpers/258550_refs.txt" -dir "${ROOT}/Rust/${OS}"

	# Show me all you've got baby
	mono "${ROOT}/Tools/Helpers/Publicizer.exe" \
		--input "${ROOT}/Rust/${OS}/RustDedicated_Data/Managed/Assembly-CSharp.dll"

	mono "${ROOT}/Tools/Helpers/Publicizer.exe" \
		--input "${ROOT}/Rust/${OS}/RustDedicated_Data/Managed/Rust.Clans.Local.dll"

	mono "${ROOT}/Tools/Helpers/Publicizer.exe" \
		--input "${ROOT}/Rust/${OS}/RustDedicated_Data/Managed/Facepunch.Network.dll"
done

dotnet restore "${ROOT}/Carbon.Core" --nologo