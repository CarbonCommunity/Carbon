#!/usr/bin/env bash
BASE="$(cd -- "$(dirname "$0")" >/dev/null 2>&1; pwd -P)"
exec "${BASE}/build.sh" Debug