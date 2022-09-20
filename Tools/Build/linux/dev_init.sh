#!/usr/bin/env bash
set -e

#  ______ _______ ______ ______ _______ _______ 
# |      |   _   |   __ \   __ \       |    |  |
# |   ---|       |      <   __ <   -   |       |
# |______|___|___|___|__|______/_______|__|____|
#               https://tinyurl.com/carbon-core                                  

BASE="$(cd -- "$(dirname "$0")" >/dev/null 2>&1; pwd -P)"
ROOT="$(realpath "${BASE}/../../../")"

# Inits and downloads the submodules
git submodule init
git submodule update

# Build Steam Downloading Utility
dotnet restore ${ROOT}/Tools/DepotDownloader --force
dotnet clean   ${ROOT}/Tools/DepotDownloader --configuration Release
dotnet build   ${ROOT}/Tools/DepotDownloader --configuration Release --no-incremental

# Build .NET Assembly stripper, publicizer and general utility tool
dotnet restore ${ROOT}/Tools/NStrip --force
dotnet clean   ${ROOT}/Tools/NStrip --configuration Release
dotnet build   ${ROOT}/Tools/NStrip --configuration Release --no-incremental

# Keeping Unity DoorStop out of the game for now due to the more
# complex build process.

# Download rust binary libs
exec ${BASE}/dev_update.sh