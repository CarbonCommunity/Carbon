![Carbon](https://i.imgur.com/dUINq6H.png)
[![.NET](https://github.com/raulssorban/Carbon.Core/actions/workflows/dotnet.yml/badge.svg)](https://github.com/raulssorban/Carbon.Core/actions/workflows/dotnet.yml)

A concept that's becoming reality. **Carbon** is a platform in the form of a DLL which gets added under your Rust server, in the *HarmonyMods* folder. 

## Insight
This project is designed to work as close as the way Oxide does, with slight interface adjustments but with the sole purpose of allowing most Oxide-dedicated plugins, work in **Carbon**'s environment.

**Follow the development roadmap [here](https://trello.com/b/FMTfHkSg/carboncore).**

## Features
* Extremely lightweight, stripped out of additional, unrelated-to-Rust processes
* Familiar folder and plugin-making structure, with the intention to have the system run almost any kind of Oxide plugin
* Permissions system
* Light Hook system

## Similarities
You'll quickly become familiar with the folder structure. Instead of **root/oxide/config** or **root/oxide/plugins** you now have **root/carbon/plugins**, etc. In the plugins folder you must add your DLLs and/or CS files. This might change in the future to organise and split the two types - and yes, both DLLs and script files will be supported.

### Hooks
We've got a very specific and select amount of hooks available by default. Will be working on adding tools for developers to - by the help of Harmony - inject code at specific parts in the original Rust runtime assembly. 

You may already do so, but it is a chore. My goal is to try and minimise modifying your original Oxide plugins to fit working with Carbon.
* OnPluginLoaded
* OnServerSave
* OnPLayerDisconnected
* OnPlayerConnected
* OnServerInitialized
* OnServerShutdown
* OnUserGroupAdded
* OnUserGroupRemoved
* OnUserPermissionGranted
* OnUserPermissionRevoked
* OnGroupPermissionGranted
* OnGroupPermissionRevoked

### Commands
**Carbon** comes with the following built in commands. They all have the prefix **_c.*_**.
* **c.version**: Prints Carbon's current version.
* **c.list**: Prints all loaded mods and plugins in said mods (you may have DLLs with multiple Oxide plugins inside).
* **c.reload**: Unloads all plugins and re-loads them from the **root/carbon/plugins** folder.
* **c.find**: Searches and filters through all the available commands processed by **Carbon**.

### Installation
Follow the following steps to understand how to integrate **Carbon** into your server. This is very early development, so I advise you to test it out first and make sure things work. If they don't, use the [issues](https://github.com/raulssorban/Carbon.Core/issues) tab. For testing, I do this:

1. Download/clone **Carbon.Core** and compile it yourself. Grab *only* the Carbon0.0.0.dll file out of your Release/Debug directory. If [releases](https://github.com/raulssorban/Carbon.Core/releases) are available, you may directly use those.
1. Add it in the *root/HarmonyMods* folder. If the server's already live, execute the following command to load **Carbon** up: **harmony.load Carbon0.0.0.dll**
1. Once loaded, you'll notice that **Carbon**'s booting up in the console.

**Note:** *Replace the 0.0.0 values with the major/minor/build versions of the build.*
