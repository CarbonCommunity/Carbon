![Carbon](https://i.imgur.com/sG6X07A.jpg)

<p align="center">
  <a href="https://github.com/Carbon-Modding/Carbon.Core/actions/workflows/develop-build.yml"><img src="https://github.com/Carbon-Modding/Carbon.Core/actions/workflows/develop-build.yml/badge.svg"></a>
  <a href="https://github.com/Carbon-Modding/Carbon.Core/actions/workflows/staging-build.yml"><img src="https://github.com/Carbon-Modding/Carbon.Core/actions/workflows/staging-build.yml/badge.svg"></a>
  <a href="https://github.com/Carbon-Modding/Carbon.Core/actions/workflows/production-build.yml"><img src="https://github.com/Carbon-Modding/Carbon.Core/actions/workflows/production-build.yml/badge.svg?branch=production"></a>
</p>


A concept that's becoming reality.
**Carbon** is a platform in the form of a DLL which gets added under your Rust server, in the *HarmonyMods* folder. 

**Join our official [Discord server](https://discord.gg/eXPcNKK4yd) for more frequent development info, discussions and future plans.**

## Documentation

**For more in-depth Carbon documentation, from builds and deployment, check [here](https://carboncommunity.gitbook.io/docs).**

Find all currently available hooks **[here](https://carboncommunity.gitbook.io/docs/core/hooks/carbon-hooks)**. We're open for your support to add any missing hooks that you want **[here](https://carboncommunity.gitbook.io/docs/core/hooks/incompatible-hooks)**.

## Features
* Extremely lightweight, stripped out of additional, unrelated-to-Rust processes
* Seamless transition from Oxide to Carbon
* Permissions system
* Hook system
* MySQL & SQLite support
* DRM support
* Chat, Console and Covalence support
* Built in Carbon Modules (internal plugins) that help your server for the better
* Carbon-only methods that speed up and better the performance of your plugins
* Integrated RustEdit extensions (Module)
* Integrated Stack manager (Module)
* Integrated Gather manager (Module)
* Embedded error/exception Demystifier which shows accurate error outputs
* Very fast & helpful error handling for plugin compilation

## Installation

### Flavors
Carbon has three different main flavors.

- 🥇 [Production]: the most stable version,
- 🥈 [Staging]: the preview version, could be unstable,
- 🥉 [Development]: the bleeding edge, where new things are born 🍼.

In general most people is advised to get the latest stable version which is the only one considered to be production ready.

### How to install
1. Download the latest version of Carbon from Carbon's Github [release page][2].
2. Unzip the archive to the root of your Rust Dedicated Server.
3. Restart the server and enjoy 🎉.

⚠️ If you are installing on a 🐧 Linux host you will need to execute the `carbon/tools/environment.sh` script before starting your server.
This script will prepare the `LD_LIBRARY_PATH` and `LD_PRELOAD` env to execute Unity Doorstop automatically.
Update your scripts to always source `source carbon/tools/environment.sh` before starting the game server.

### Folder structure
This is the example of a default Carbon installation, the full folder structure will get created when starting the rust dedicated server for the first time after the installation.

```
.
├── config.json
├── configs
├── data
│   ├── oxide.groups.data
│   └── oxide.users.data
├── harmony
├── lang
├── logs
│   ├── Carbon.Doorstop.log
│   ├── Carbon.Loader.log
│   └── (...)
├── managed
│   ├── Carbon.Doorstop.dll
│   ├── Carbon-Unix.dll
│   └── lib
│       ├── 1Harmony.dll
│       ├── Ben.Demystifier.dll
│       └── (...)
├── modules
│   ├── RustEdit.Ext
│   │   ├── config.json
│   │   └── data.json
│   └── (...)
├── plugins
├── reports
├── temp
└── tools
    ├── environment.sh
    └── publicizer.sh
```

## Compilation

This following instructions were written for 🪟 Windows environments but Carbon can be built on 🐧 Linux hosts as well.
The project has been successfully built using:
  - Visual Studio 2019/2022 (🪟)
  - Visual Studio Code (🪟, 🐧)

### Preparing the buildroot

1. ⬇️ Clone [the project][1] on your machine.
2. 📂 Go to the `Tools\Build\win` folder.
3. 👟 Run `bootstrap.bat` for it to download all Rust-related DLLs.
4. 📒 Open the solution found in `Carbon.Core\Carbon.Core.sln`.
5. 🚀 Develop, build and have fun.

## Exporting

To export your own patches locally, run the `Tools\Build\win\build_debug.bat` script.
This will create a `Releases` folder on project's root with the `.dll` and `.zip` files. 

[1]: https://github.com/Carbon-Modding/Carbon.Core
[2]: (hhttps://github.com/Carbon-Modding/Carbon.Core/releases/latest)

[production]: https://github.com/Carbon-Modding/Carbon.Core/releases/latest
[staging]: https://github.com/Carbon-Modding/Carbon.Core/releases/tag/staging_build
[development]: https://github.com/Carbon-Modding/Carbon.Core/releases/tag/develop_build
