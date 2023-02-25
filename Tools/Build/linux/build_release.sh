#!/usr/bin/env bash
BASE="$(cd -- "$(dirname "$0")" >/dev/null 2>&1; pwd -P)"

"${BASE}/build.sh" Release ${1}
"${BASE}/build.sh" ReleaseUnix ${1}