#!/usr/bin/env bash

# Carbon with Oxide: also ships the Oxide compatibility package and the generated Oxide hooks
set -euo pipefail

BASE="$(cd -- "$(dirname "$0")" >/dev/null 2>&1 && pwd -P)"

"${BASE}/build.sh" DebugUnix EDGE edge_build -oxide "$@"
