![GitHub-Mark-Light](./.press/logo_light.png#gh-dark-mode-only)![GitHub-Mark-Dark](./.press/logo_dark.png#gh-light-mode-only)

<p align="center">
  <a href="https://github.com/Carbon-Modding/Carbon.Core/actions/workflows/develop-build.yml"><img src="https://github.com/Carbon-Modding/Carbon.Core/actions/workflows/develop-build.yml/badge.svg"></a>
  <a href="https://github.com/Carbon-Modding/Carbon.Core/actions/workflows/staging-build.yml"><img src="https://github.com/Carbon-Modding/Carbon.Core/actions/workflows/staging-build.yml/badge.svg"></a>
  <a href="https://github.com/Carbon-Modding/Carbon.Core/actions/workflows/production-build.yml"><img src="https://github.com/Carbon-Modding/Carbon.Core/actions/workflows/production-build.yml/badge.svg?branch=production"></a>
</p>

## Introduction
Carbon is an anticipated system designed for lower-to-higher-end **Rust** servers with top priority for best performance and memory optimization, which is compatible with most - if not all - **Oxide** plugins currently out in the market, and a lot more features!

Join our official [Discord server][discord] for more frequent development info, discussions and future plans.

## Documentation

For more in-depth Carbon documentation, from builds and deployment, check [here][documentation].

Find all currently available hooks [here][5].
We're open for your support to add any missing hooks that you want [here][6].

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

## Running Carbon

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

#### Pterodactyl
1. Download [this file][3] and save it locally as `egg-rust-carbon.json`.
2. Open your Pterodactyl admin panel and go to the `Nests` section.
3. Click `Import Egg` and browse for the file you've just downloaded.
4. Select `Rust` as your `Associated Nest` and import.

From here you can build a new Rust server with Carbon as your selected egg.

The source code for this egg is also [available on Github][4].
For help and support, join [Carbon's Discord server][discord] and `@RustRadio`.

## Building Carbon

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

When building locally there a set of scripts that can help you during the development cycle, those scripts are located on the `Tools\Build\` folder, we have windows and linux scripts available.
To export your own artifacts locally, run the `Tools\Build\win\build_debug.bat` script.
This will create a `Releases` folder on project's root with the `.dll` and `.zip` files. 

[1]: https://github.com/Carbon-Modding/Carbon.Core
[2]: https://github.com/Carbon-Modding/Carbon.Core/releases/latest
[3]: https://raw.githubusercontent.com/jondpugh/Carbon-Ptero/main/egg-rust-carbon.json
[4]: https://github.com/jondpugh/Carbon-Ptero
[5]: https://carboncommunity.gitbook.io/docs/core/hooks/carbon-hooks
[6]: https://carboncommunity.gitbook.io/docs/core/hooks/incompatible-hooks

[production]: https://github.com/Carbon-Modding/Carbon.Core/releases/latest
[staging]: https://github.com/Carbon-Modding/Carbon.Core/releases/tag/staging_build
[development]: https://github.com/Carbon-Modding/Carbon.Core/releases/tag/develop_build

[discord]: https://discord.gg/eXPcNKK4yd
[documentation]: https://carboncommunity.gitbook.io/docs
