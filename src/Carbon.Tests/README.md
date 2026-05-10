# Carbon.Tests

Automated test runner suite for testing Carbon.
Mainly used inside [CI/CD](../../.github/workflows/common-test.yml)

## Required Env Variables

```dotenv
# Can be an absolute path to any folder that should be used for the server
WorkingDir=.dev

# Branch name of the Rust server to download
BranchName=public

# Accepts a url to a zip or tar archive. Also supports local file uri
CarbonDownloadZipUrl=https://github.com/CarbonCommunity/Carbon/releases/download/edge_build/Carbon.Windows.Debug.zip
# CarbonDownloadZipUrl=https://github.com/CarbonCommunity/Carbon/releases/download/edge_build/Carbon.Linux.Debug.tar.gz
# CarbonDownloadZipUrl=file:///home/user/p/Carbon/release/Carbon.Linux.Debug.tar.gz

# Skip Depot Downloader hit if Rust server is already present
ForDebug__SkipRustServerIfPresent=false

# Skip running the Rust server entirely
ForDebug__NoRustServerRun=false

# Skip Carbon if the directory is already present
ForDebug__SkipCarbonIfPresent=false

# Optional test compiler opt-outs. When true, Carbon.Tests adds the matching
# TESTS_NO_* symbol to carbon/config.json before the Tests plugin compiles.
Tests__NoHooks=false
Tests__NoLogging=false
Tests__NoPermission=false
Tests__NoPermissionSqlMigration=false
Tests__NoProfiler=false
Tests__NoWebRequest=false
```

For dev purposes, you can create a `.env` file (or rename `.env.example` to `.env`) and add it as an environment variable file inside the run configuration,
or use the `Carbon.Tests (DEV+ENV)` run config (still needs `.env`).
