#!/usr/bin/env bash

set -euo pipefail

BASE="$(cd -- "$(dirname "$0")" >/dev/null 2>&1 && pwd -P)"
SUPERROOT="${BASE}/tools/build/linux"

"${SUPERROOT}/bootstrap.sh"
"${SUPERROOT}/build_debug_noarchive.sh"
