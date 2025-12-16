# Carbon.TestRunner

Automated test runner suit for testing carbon.
Mainly used inside [CI/CD](../../.github/workflows/common-test.yml)

## Required Env Variables

```dotenv
WorkingDir=.dev
BranchName=public
CarbonDownloadZipUrl=https://github.com/CarbonCommunity/Carbon/releases/download/edge_build/Carbon.Windows.Debug.zip

# Skip Depot Downloader hit if Rust server is already present
ForDebug__SkipRustServerIfPresent=false

# Skip Rust server run at all
ForDebug__NoRustServerRun=false
```

For dev purposes, you can create `.env` file and add it as Env variable inside run configuration,
or use `Carbon.TestRunner (DEV+ENV)` run config (still need `.env`)
