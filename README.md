![Carbon Light Logo](https://raw.githubusercontent.com/CarbonCommunity/.github/refs/heads/main/profile/press/carbonlogo_w.png#gh-dark-mode-only)
![Carbon Dark Logo](https://raw.githubusercontent.com/CarbonCommunity/.github/refs/heads/main/profile/press/carbonlogo_b.png#gh-light-mode-only)

<p align="center">
  <a href="https://github.com/CarbonCommunity/Carbon/releases/tag/edge_build"><img src="https://github.com/CarbonCommunity/Carbon/actions/workflows/edge-build.yml/badge.svg" /></a>
  <a href="https://github.com/CarbonCommunity/Carbon/releases/tag/experimental_build"><img src="https://github.com/CarbonCommunity/Carbon/actions/workflows/experimental-build.yml/badge.svg" /></a>
  <a href="https://github.com/CarbonCommunity/Carbon/releases/latest"><img src="https://github.com/CarbonCommunity/Carbon/actions/workflows/production-build.yml/badge.svg" /></a>
  <br />
    <a href="https://github.com/CarbonCommunity/Carbon/releases/tag/rustbeta_staging_build"><img src="https://github.com/CarbonCommunity/Carbon/actions/workflows/rust-staging-build.yml/badge.svg" /></a>
    <a href="https://github.com/CarbonCommunity/Carbon/releases/tag/rustbeta_release_build"><img src="https://github.com/CarbonCommunity/Carbon/actions/workflows/rust-release-build.yml/badge.svg" /></a>
    <a href="https://github.com/CarbonCommunity/Carbon/releases/tag/rustbeta_aux01_build"><img src="https://github.com/CarbonCommunity/Carbon/actions/workflows/rust-aux01-build.yml/badge.svg" /></a>
    <a href="https://github.com/CarbonCommunity/Carbon/releases/tag/rustbeta_aux02_build"><img src="https://github.com/CarbonCommunity/Carbon/actions/workflows/rust-aux02-build.yml/badge.svg" /></a>
  <br />
  <a href="https://github.com/CarbonCommunity/Carbon/releases/tag/profiler_build"><img src="https://github.com/CarbonCommunity/Carbon/actions/workflows/profiler-build.yml/badge.svg" /></a>
  <a href="https://github.com/CarbonCommunity/Carbon/blob/main/LICENSE"><img alt="GitHub" src="https://img.shields.io/github/license/CarbonCommunity/Carbon" /></a>
  <a href="https://www.nuget.org/packages/Carbon.Community"><img alt="NuGet" src="https://img.shields.io/nuget/v/Carbon.Community.svg" /></a>
  <a href="https://github.com/GameServerManagers/LinuxGSM/releases/latest"><img src="https://img.shields.io/badge/LinuxGSM-v23.2.0-informational" /></a>
  <hr />
</p>

Carbon is a self-updating, lightweight, intelligent mod loader for Rust utilizing the latest C# and Harmony for the best performance and stability possible. It ships in two flavors: **Carbon**, a standalone framework with its own plugin API, and **Carbon with Oxide**, which adds backward compatibility with Oxide plugins and extensions on top.

Carbon has all the creature comforts you need to run your server, such as a permission system, user system, and so much more. Carbon is developed by experienced developers and server owners working to take the tedium out of hosting servers and make configuration and setup seamless with an integrated GUI in-game to manage everything!

## :package: Download
Start using Carbon today, download the latest version from our [releases page][production].
We also provide a [quick start script][quick-start] to get your server running in minutes, available for Windows and Linux.

## :blue_book: Documentation
For more in-depth Carbon documentation, from builds and deployment, check [here][documentation].
Find all currently available hooks [here][hooks].
If you are a developer take a look at our [Developer Documentation][developer-doc].

## :question: Support
Join our official [Discord server][discord] for support, more frequent development info, discussions and future plans.

## ⚙️ Development
To help us and work with us on the project, or get started with Carbon's structure, follow this for understanding how it works.
The project is split in various essential and mostly independent [components](./src/Carbon.Components), making it easier to organize and follow.

### Getting Started
Upon cloning Carbon, all you need to execute is [`setup.bat`](./setup.bat) to initialize the entire project and its dependencies.

### Branches
The following branches are shared across all Component and Hook projects and synchronized accordingly with the main project ([this one](https://github.com/CarbonCommunity/Carbon)) and its branches.
- `main` branch is the primary base branch of Carbon. It's used to be merged into Rust beta branches as well as the `production` (live) branch.
  - Used against Rust `public` branch.
  - Public [build](https://github.com/CarbonCommunity/Carbon/releases/tag/edge_build), gets updated every time we commit changes to Carbon.
- `experimental` branch is usually synced up with `main` and has experimental features that may or may not be brought into `main`.
  - Used against Rust `release` branch (usually).
  - Public [build](https://github.com/CarbonCommunity/Carbon/releases/tag/preview_build), gets manually triggered to get updated, not very often.
- `production` branch is the primary (live) branch of Carbon and [main release](https://github.com/CarbonCommunity/Carbon/releases/tag/production_build) is built off of.
  - Used against Rust `public` branch or `release` on Rust wipe day.
  - Public [build](https://github.com/CarbonCommunity/Carbon/releases/tag/production_build), gets updated twice times a month, excluding important hotfix patches.
- `rust_beta/staging` is often times synced up with `main` and has changes that might come to Rust `release|public` branch in a future update, which can contain mandatory changes to Carbon to address the Rust changes.
  - Used against Rust `staging` branch.
  - Public [build](https://github.com/CarbonCommunity/Carbon/releases/tag/rustbeta_staging_build), gets updated a few times a month. Usually only hooks get updated. Mainly merged from `develop`.
- `rust_beta/aux01` is often times synced up with `rust_beta/staging` and is minimally supported, just enough to be able to run Carbon on AUX01. This Rust branch is extremely unstable and contains things that may or may not ever be merged to the main version of Rust.
  - Used against Rust `aux01` branch. The staging branch beta client is not always available.
  - Public [build](https://github.com/CarbonCommunity/Carbon/releases/tag/rustbeta_aux01_build), gets updated a few times a month. Usually only hooks get updated.
- `rust_beta/aux02` is often times synced up with `rust_beta/staging` and is minimally supported, just enough to be able to run Carbon on AUX02. This Rust branch is extremely unstable and contains things that may or may not ever be merged to the main version of Rust.
  - Used against Rust `aux02` branch. The staging branch beta client is not always available.
  - Public [build](https://github.com/CarbonCommunity/Carbon/releases/tag/rustbeta_aux02_build), gets updated a few times a month. Usually only hooks get updated.

### Components
- [Carbon.Preloader](./src/Carbon.Components/Carbon.Preloader): Runtime preloader of dependencies and responsible for the self-updating process. It invokes Carbon.Startup.
- [Carbon.Startup](./src/Carbon.Components/Carbon.Startup): Handles in-memory Rust assembly patching and publicizing and exporting if Developer Mode is enabled in the config. Applies the startup tasks declared by packages.
- [Carbon.Bootstrap](./src/Carbon.Components/Carbon.Bootstrap): Initial Carbon boot in the primary app-domain. Installs the core services, loads packages, then Carbon.
- [Carbon.Common](./src/Carbon.Components/Carbon.Common): The core of Carbon: the plugin API (`Carbon.Plugins`), plugin sources and compiler, hooks, managers, modules, CUI and tools.
- [Carbon.SDK](./src/Carbon.Components/Carbon.SDK): Low-level shared types (events, commands, hook attributes, addon contracts) used by components that can't depend on Carbon.Common.
- [Carbon.Modules](./src/Carbon.Components/Carbon.Modules): Carbon optional modules expanding functionality, enhanced QoL and tools.
- [Carbon.Compat](./src/Carbon.Components/Carbon.Compat): Makes Rust HarmonyMods loadable under Carbon. Originally the Carbon Compatibility Loader written by Patrette (community member).
- [Carbon.Oxide](./src/Carbon.Components/Carbon.Oxide): The Oxide compatibility package (only in "Carbon with Oxide" builds): Oxide's API, plugin source translation, extension conversion and covalence hooks.
- [Carbon.Test](./src/Carbon.Components/Carbon.Test): Integral implementation for automated testing rules and events.

### Hooks
Carbon's hooks are managed in [`src/Carbon.Hooks`](./src/Carbon.Hooks):
- [Carbon.Hooks.Base](./src/Carbon.Hooks/Carbon.Hooks.Base): Includes ground level dynamic and static patching instructions supplementing necessary events for Carbon's own runtime, including the plugin lifecycle hooks.
- [Carbon.Hooks.Community](./src/Carbon.Hooks/Carbon.Hooks.Community): Community curated patches and hooks.
- [Carbon.Hooks.Generator](./src/Carbon.Hooks/Carbon.Hooks.Generator): Generates Carbon Harmony hooks based on an Oxide `.opj` file.
- [Carbon.Hooks.Oxide](./src/Carbon.Hooks/Carbon.Hooks.Oxide): The generated Oxide hooks, only shipped in "Carbon with Oxide" builds.

### Building
To locally build Carbon from scratch, execute the [`setup.bat`](./setup.bat) or [`setup.sh`](./setup.sh) file, and find the results in the root of Carbon, under `./release`.

To build "Carbon with Oxide", use the `*_oxide` scripts in [`tools/build`](./tools/build) (e.g. `build_debug_oxide.bat`), or pass `-oxide` to the build runner. Those archives carry an `.Oxide` suffix.

See [`docs/CARBON-3.md`](./docs/CARBON-3.md) for the 3.0 architecture and migration notes.

## :heart: Sponsor

If you would like to [sponsor][patreon] the project the best way is to use [Patreon].

We would like to thank everyone who sponsors us.

[hooks]: https://carbonmod.gg/references/hooks/
[discord]: https://discord.gg/carbonmod
[documentation]: https://carbonmod.gg/
[patreon]: https://patreon.com/CarbonMod
[production]: https://github.com/CarbonCommunity/Carbon/releases/tag/production_build
[quick-start]: https://github.com/CarbonCommunity/Carbon.QuickStart
[developer-doc]: https://carbonmod.gg/devs/creating-your-project
