#!/usr/bin/env bash

set -euo pipefail

BASE="$(cd -- "$(dirname "$0")" >/dev/null 2>&1 && pwd -P)"

"${BASE}/_runner.sh" Tools/Build/runners/git.cs "$@"
