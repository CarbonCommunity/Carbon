#!/usr/bin/env bash

###
### Copyright (c) 2022 Carbon Community
### All rights reserved
###

SCRIPT=$( cd -- "$( dirname -- "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )
source ${SCRIPT}/carbon/tools/environment.sh
${SCRIPT}/RustDedicated "$@"