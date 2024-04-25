#!/usr/bin/env bash

###
### Copyright (c) 2022-2023 Carbon Community
### All rights reserved
###

# Get the directory of the executable
CARBONENV_SCRIPT=$( cd -- "$( dirname -- "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )
CARBONENV_BASEDIR=$(realpath "${CARBONENV_SCRIPT}/../../")

# Docker workaround
export TERM=xterm

# Prepare unity doorstop
export DOORSTOP_ENABLED=1
export DOORSTOP_TARGET_ASSEMBLY="${CARBONENV_BASEDIR}/carbon/managed/Carbon.Preloader.dll"

# Prepare the environment
export LD_PRELOAD="${CARBONENV_BASEDIR}/libdoorstop.so"
export LD_LIBRARY_PATH="${CARBONENV_BASEDIR}::${CARBONENV_BASEDIR}/RustDedicated_Data/Plugins/x86_64"
