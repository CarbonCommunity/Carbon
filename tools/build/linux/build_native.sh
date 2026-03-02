#!/usr/bin/env bash

set -euo pipefail

BASE="$(cd -- "$(dirname "$0")" >/dev/null 2>&1 && pwd -P)"
ROOT="$(realpath "${BASE}/../../../")"
NATIVE_DIR="${ROOT}/src/Carbon.Native"

cd "${NATIVE_DIR}"

rustup target add x86_64-unknown-linux-gnu
rustup target add x86_64-pc-windows-gnu

if command -v cross >/dev/null 2>&1; then
  cross build -r --target x86_64-unknown-linux-gnu
  cross build -r --target x86_64-pc-windows-gnu
else
  cargo build -r --target x86_64-unknown-linux-gnu
  cargo build -r --target x86_64-pc-windows-gnu
fi
