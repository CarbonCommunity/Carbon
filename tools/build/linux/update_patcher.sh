#!/usr/bin/env bash

set -euo pipefail

BASE="$(cd -- "$(dirname "$0")" >/dev/null 2>&1 && pwd -P)"
BRANCH="${1:-release}"
OUTPUT="${2:-xx}"

"${BASE}/_runner.sh" Tools/Build/runners/patcher_setup.cs "${BRANCH}" "${OUTPUT}"
