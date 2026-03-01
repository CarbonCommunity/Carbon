#!/usr/bin/env bash

set -euo pipefail

BASE="$(cd -- "$(dirname "$0")" >/dev/null 2>&1 && pwd -P)"

"${BASE}/build_native.sh"

export VERSION="${VERSION:-2.0.0}"

"${BASE}/_runner.sh" Tools/Build/runners/profiler.cs Debug HARMONYMOD edge_build "$@"
"${BASE}/_runner.sh" Tools/Build/runners/profiler.cs DebugUnix HARMONYMOD edge_build "$@"
