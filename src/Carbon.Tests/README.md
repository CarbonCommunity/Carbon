# Carbon.Tests

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

# Skip Carbon if the directory is already present
ForDebug__SkipCarbonIfPresent=false
```

For dev purposes, you can create `.env` file (or rename `.env.example` to `.env`) and add it as Env variable inside run configuration,
or use `Carbon.Tests (DEV+ENV)` run config (still need `.env`)
