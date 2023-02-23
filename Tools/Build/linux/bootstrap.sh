#!/usr/bin/env bash

###
### Copyright (c) 2022 Carbon Community 
### All rights reserved
###
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

# Inits and downloads the submodules
git submodule init
git submodule update

## Changes the assembly name for HamonyLib
#HARMONYDIR="${ROOT}/Tools/HarmonyLib/Harmony"
#sed -i 's/0Harmony/1Harmony/' "${HARMONYDIR}/Harmony.csproj"

for TOOL in DepotDownloader NStrip HarmonyLib; do
  dotnet restore "${ROOT}/Tools/${TOOL}" --verbosity quiet --nologo --force
  dotnet clean   "${ROOT}/Tools/${TOOL}" --verbosity quiet --configuration Release --nologo
  dotnet build   "${ROOT}/Tools/${TOOL}" --verbosity quiet --configuration Release --no-restore --no-incremental
done

# Download rust binary libs
"${BASE}/update.sh" public