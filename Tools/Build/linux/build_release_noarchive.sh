#!/usr/bin/env bash

set -euo pipefail

BASE="$(cd -- "$(dirname "$0")" >/dev/null 2>&1 && pwd -P)"

"${BASE}/build.sh" Release EDGE edge_build -noarchive "$@"
