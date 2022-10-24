#!/usr/bin/env bash

###
### Copyright (c) 2022 Carbon Community
### All rights reserved
###

set -e

# Get the directory of the executable
SCRIPT=$( cd -- "$( dirname -- "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )
BASEDIR="${SCRIPT}/../../"

# Prepare the environment
export TERM=xterm
export DOORSTOP_ENABLED=1
export DOORSTOP_TARGET_ASSEMBLY="${BASEDIR}/carbon/managed/Carbon.Doorstop.dll"

if [ -z "${LD_LIBRARY_PATH}" ]; then
    export LD_LIBRARY_PATH="${BASEDIR}:${BASEDIR}/RustDedicated_Data/Plugins/x86_64"
else
    export LD_LIBRARY_PATH="${BASEDIR}:${BASEDIR}/RustDedicated_Data/Plugins/x86_64:${LD_LIBRARY_PATH}"
fi

if [ -z "${LD_PRELOAD}" ]; then
    export LD_PRELOAD="libdoorstop.so"
else
    export LD_PRELOAD="libdoorstop.so:${LD_PRELOAD}"
fi