#!/usr/bin/env bash

BASE="$(cd -- "$(dirname "$0")" >/dev/null 2>&1; pwd -P)"

"${BASE}/build.sh" Release
"${BASE}/build.sh" ReleaseUnix

HOME=$(pwd)
ROOT=$(dirname "$(dirname "$(dirname "$HOME")")")

if [ ! -d "$ROOT/Release/.native" ]; then
	mkdir -p "$ROOT/Release/.native"
fi

cd "$ROOT/Carbon.Core/Carbon.Native" || exit

cargo build

cp "$ROOT/Carbon.Core/Carbon.Native/target/debug/CarbonNative.dll" "$ROOT/Release/.native/" || exit
