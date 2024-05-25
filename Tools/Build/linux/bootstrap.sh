#!/usr/bin/env bash

###
### Copyright (c) 2022-2023 Carbon Community 
### All rights reserved
###
set -e

cat <<EOF
  ______ _______ ______ ______ _______ _______ 
 |      |   _   |   __ \   __ \       |    |  |
 |   ---|       |      <   __ <   -   |       |
 |______|___|___|___|__|______/_______|__|____|
                          discord.gg/carbonmod

EOF

BASE="$(cd -- "$(dirname "$0")" >/dev/null 2>&1; pwd -P)"
ROOT="$(realpath "${BASE}/../../../")"

# Inits and downloads the submodules
git -C "${ROOT}" submodule init
git -C "${ROOT}" submodule update

CURRENT_BRANCH=$(git branch --show-current)

echo Handling component submodules..
for TOOL in Carbon.Core/Carbon.Components/Carbon.Bootstrap Carbon.Core/Carbon.Components/Carbon.Common Carbon.Core/Carbon.Components/Carbon.Compat Carbon.Core/Carbon.Components/Carbon.Modules Carbon.Core/Carbon.Components/Carbon.Preloader Carbon.Core/Carbon.Components/Carbon.SDK Carbon.Core/Carbon.Hooks/Carbon.Hooks.Base Carbon.Core/Carbon.Hooks/Carbon.Hooks.Oxide Carbon.Core/Carbon.Hooks/Carbon.Hooks.Community; do
  echo Updating ${TOOL}
  cd ${ROOT}/${TOOL}
  # git fetch --all --unshallow > /dev/null
  # git checkout -b ${CURRENT_BRANCH} > /dev/null
  # git fetch > /dev/null
  # git pull > /dev/null
  echo done.
done
echo Finished - handling component submodules.

echo Building submodules..
for TOOL in DepotDownloader; do
  echo Build "${TOOL}"
  dotnet restore "${ROOT}/Tools/${TOOL}" --verbosity quiet --nologo --force > /dev/null
  dotnet clean   "${ROOT}/Tools/${TOOL}" --verbosity quiet --configuration Release --nologo > /dev/null
  dotnet build   "${ROOT}/Tools/${TOOL}" --verbosity quiet --configuration Release --no-restore --no-incremental > /dev/null
  echo done.
done

# Download rust binary libs
"${BASE}/update.sh"

# Don't track changes to this file
git -C "${ROOT}" update-index --assume-unchanged "${ROOT}/Tools/Helpers/doorstop_config.ini"