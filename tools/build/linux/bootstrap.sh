#!/usr/bin/env bash

set -euo pipefail

BASE="$(cd -- "$(dirname "$0")" >/dev/null 2>&1 && pwd -P)"
TARGET="${1:-release}"

"${BASE}/_runner.sh" Tools/Build/runners/bootstrap.cs
"${BASE}/_runner.sh" Tools/Build/runners/update.cs "${TARGET}"
