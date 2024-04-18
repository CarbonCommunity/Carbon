#!/usr/bin/env bash

BASE="$(cd -- "$(dirname "$0")" >/dev/null 2>&1; pwd -P)"

"${BASE}/build.sh" Release
"${BASE}/build.sh" ReleaseUnix

HOME=$(pwd)
ROOT=$(dirname "$(dirname "$(dirname "$HOME")")")

cd "$ROOT/Carbon.Core/Carbon.Native" || exit

cargo build
