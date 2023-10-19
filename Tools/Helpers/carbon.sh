#!/usr/bin/env bash

###
### Copyright (c) 2022-2023 Carbon Community
### All rights reserved
###

CARBON_INIT_SCRIPT=$( cd -- "$( dirname -- "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )
source "${CARBON_INIT_SCRIPT}/carbon/tools/environment.sh"
"${CARBON_INIT_SCRIPT}/RustDedicated" "$@"
