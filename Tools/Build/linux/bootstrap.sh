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

echo Handling component submodules..
for TOOL in Carbon.Core/Carbon.Compat; do
  echo Updating ${TOOL}
  cd ${ROOT}/${TOOL}
  git clean -fd
  git fetch
  git pull . main
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
"${BASE}/update.sh" release

# Don't track changes to this file
git -C "${ROOT}" update-index --assume-unchanged "${ROOT}/Tools/Helpers/doorstop_config.ini"