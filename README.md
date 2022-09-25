![Carbon](https://i.imgur.com/sG6X07A.jpg)
[![🦺 Validate build status](https://github.com/Carbon-Modding/Carbon.Core/actions/workflows/alpha-build.yml/badge.svg)](https://github.com/Carbon-Modding/Carbon.Core/actions/workflows/alpha-build.yml)

A concept that's becoming reality. **Carbon** is a platform in the form of a DLL which gets added under your Rust server, in the *HarmonyMods* folder. 

**Follow the development roadmap [here](https://trello.com/b/FMTfHkSg/carboncore).**
**Join our official [Discord server](https://discord.gg/eXPcNKK4yd) for discussions.**

## Documentation
**For server-owner related or development questions, check [here](https://carbon-modding.gitbook.io/docs).**

## Features
* Extremely lightweight, stripped out of additional, unrelated-to-Rust processes
* Familiar folder and plugin-making structure, with the intention to have the system run almost any kind of Oxide plugin
* Permissions system
* Light Hook system

## Compilation
This is ideally written for Windows environments.
1. Clone the project on your machine.
1. Go to the Tools\Build\win folder.
1. Run dev_init.bat for it to download all Rust-related DLLs.
1. Open the solution found in Carbon.Core.
1. Develop and build.

To export your own patches locally, run the Tools\Build\win\dev_release.bat file. The Releases folder will be populated in root. 
