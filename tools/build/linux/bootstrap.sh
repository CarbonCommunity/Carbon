#!/usr/bin/env bash

set -euo pipefail

BASE="$(cd -- "$(dirname "$0")" >/dev/null 2>&1 && pwd -P)"
TARGET="${1:-release}"

"${BASE}/_runner.sh" tools/build/runners/bootstrap.cs
"${BASE}/_runner.sh" tools/build/runners/update.cs "${TARGET}"
