#!/usr/bin/env bash

# Carbon Vanilla: leaves out the Oxide compatibility package and the generated Oxide hooks
set -euo pipefail

BASE="$(cd -- "$(dirname "$0")" >/dev/null 2>&1 && pwd -P)"

"${BASE}/build.sh" MinimalUnix MINIMAL edge_build -vanilla "$@"
