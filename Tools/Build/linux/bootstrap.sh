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

# Inits and downloads the submodules
git submodule init
git submodule update

# Build Steam Downloading Utility
dotnet restore ${ROOT}/Tools/DepotDownloader --nologo --force
dotnet clean   ${ROOT}/Tools/DepotDownloader --configuration Release --nologo
dotnet build   ${ROOT}/Tools/DepotDownloader --configuration Release --no-restore --no-incremental

# Build .NET Assembly stripper, publicizer and general utility tool
dotnet restore ${ROOT}/Tools/NStrip --nologo --force
dotnet clean   ${ROOT}/Tools/NStrip --configuration Release --nologo
dotnet build   ${ROOT}/Tools/NStrip --configuration Release --no-restore --no-incremental

# Keeping Unity DoorStop out of the game for now due to the more
# complex build process.

# Download rust binary libs
exec ${BASE}/update.sh