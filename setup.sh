#!/usr/bin/env bash

set -euo pipefail

BASE="$(cd -- "$(dirname "$0")" >/dev/null 2>&1 && pwd -P)"
BUILD_ROOT="${BASE}/tools/build/linux"

"${BUILD_ROOT}/bootstrap.sh"
"${BUILD_ROOT}/build_debug_noarchive.sh" "$@"
