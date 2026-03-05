#!/usr/bin/env bash

set -euo pipefail

BASE="$(cd -- "$(dirname "$0")" >/dev/null 2>&1 && pwd -P)"
ROOT="$(realpath "${BASE}/../../../")"

if [[ $# -lt 1 ]]; then
  echo "Usage: $(basename "$0") <runner-file> [args...]" >&2
  exit 1
fi

RUNNER_FILE="$1"
shift

(cd "${ROOT}" && dotnet run --project "${ROOT}/src/Carbon.Tools/Carbon.Runner" "${RUNNER_FILE}" "$@")
