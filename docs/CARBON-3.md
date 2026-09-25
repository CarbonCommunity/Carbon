# Carbon 3.0

Carbon 3.0 is a restructure of the whole project. The headline changes:

- **Two flavors.** *Carbon* is a standalone framework with no Oxide code in it. *Carbon with Oxide* adds the Oxide compatibility package on top and behaves like Carbon 2.x did.
- **A native plugin API.** `Carbon.Plugins.Plugin` → `CarbonPlugin` own permissions, lang, timers, commands, config and web requests. The member names match what plugin developers already know (`permission`, `lang`, `timer`, `cmd`, `webrequest`, `Config`, `Puts`, `PrintToChat`, `[ChatCommand]`, `[Info]`…).
- **No more processors.** The script/zip/dev-folder processors, `CarbonProcessor` and `ModuleProcessor` were replaced by `PluginSources`, `Scheduler` and `ModuleRegistry`.
- **No more `API.*` namespace.** Contracts live in `Carbon.*` namespaces, and single-implementation interfaces became concrete, public classes.
- **Extension points everywhere.** Everything Oxide-related plugs in through public hooks, so anyone can build their own packages the same way.

## Layout

| Assembly | Role |
| --- | --- |
| `Carbon.SDK` | Low-level shared types: `CarbonEvent`, `Command`, hook attributes, addon contracts, `VersionNumber`, package attributes. |
| `Carbon.Common` | Everything else: plugin API (`Carbon.Plugins`), plugin sources + compiler (`Carbon.Core`), hooks (`Carbon.Hooks`), managers (`Carbon.Managers`), modules, CUI (`Carbon.Components`). |
| `Carbon.Bootstrap` | Boots the core `Services`, loads packages, then `Carbon.dll`. |
| `Carbon` | The boot component (`Initializer`) creating `Community.Runtime`. |
| `Carbon.Compat` | HarmonyMod conversion. |
| `Carbon.Oxide` | The Oxide compatibility package (Carbon with Oxide only). |
| `Carbon.Hooks.Base` / `.Community` | Carbon's own hooks, shipped in both flavors. |
| `Carbon.Hooks.Oxide` | Generated Oxide hooks, Carbon with Oxide only. |

### Runtime systems

- `Services` (static, created at early boot): `Events`, `Commands`, `FileWatcher`, `Analytics`, `Downloads`, `Assemblies`.
- `Community.Runtime`: `PluginSources`, `HookManager` (`PatchManager`), `Scheduler`, `Modules` (`ModuleRegistry`), `Permission`, `DataFileSystem`, `Core`, `Config`. The manager properties (`Events`, `CommandManager`, …) forward to `Services`.

## Extension points

Packages are assemblies in `carbon/managed/packages`. They implement `ICarbonComponent`, load before `Carbon.dll`, and the plugin compiler references them automatically. Everything the Oxide package does goes through the hooks below:

| Hook | Purpose |
| --- | --- |
| `ModLoader.PluginBaseTypes` / `PluginNamespaces` | Accept more plugin base classes / namespaces. |
| `ScriptCompilationThread.SourceTransformers` / `ConditionalSymbols` | Rewrite plugin sources before compiling, add preprocessor symbols. |
| `PluginSources.Register(PluginSource)` | Load plugins from new formats or locations. |
| `AssemblyManager.Converters` | Rewrite third-party assemblies before they load (`AssemblyConverter`). |
| `PatchManager.HookAssemblies`, `Hooks.Updater.RemoteFiles` | Ship more hook assemblies. |
| `CommandLibrary.CallerFactories` | Bind custom caller types in command methods. |
| `LibraryLoader.Blacklist` | Keep assemblies from resolving. |
| `HarmonyConverter.AdditionalPatches`, `HarmonyPatchProcessor.PatchWhitelist.string_blacklist` | Extend HarmonyMod conversion. |
| `Permission.OnUserRefreshed`, `CarbonEvent.LibrariesInstalled` | React to the permission system. |
| `Community.CreatePermission` / `InstallLibraries` / `InstallRuntime` (virtual) | Swap core implementations. |
| `Plugin.CreateCommandLibrary` / `CreateLang` / `CreateTimers` / `CreateWebRequests` (virtual) | Per-plugin library implementations. |
| `[assembly: StartupTask]`, `[assembly: InjectField]` | Declare file tasks / Rust fields that `Carbon.Startup` applies before Rust loads. |
| `AdminModule.PluginsTab.ExternalVendorFactory`, `ImageDatabaseModule.DefaultImages` | Admin panel integrations. |

## How Oxide compatibility works

The Oxide package (`Carbon.Oxide`) contains the real Oxide-only API: `Interface`, `OxideMod`, `RustPlugin`, `CovalencePlugin`, covalence (`IPlayer`, …), the Rust/Server/Player libraries, Oxide extensions, MySql/SQLite, `Hash`, and so on.

Types Carbon provides natively (`Plugin`, `Permission`, `Lang`, `Timer`, `DynamicConfigFile`, the CUI types, …) are **not** duplicated. `OxideTypeMap` maps their Oxide names to the native ones, and two things use that map:

- `OxideSourceTranslator` rewrites plugin sources before they compile:
  - It turns `using Oxide.Game.Rust.Cui;` into aliases of the native types.
  - It rewrites fully qualified names.
  - It drops imports of namespaces that no longer exist.
  - Oxide plugins end up using Carbon's types directly, so a `[PluginReference] Plugin x` binds to native plugins too.
- The Oxide extension converter remaps precompiled Oxide extensions the same way.

`RustPlugin` derives from `CarbonPlugin`. `OxideCore` (a core plugin inside the package) re-emits Carbon's `OnPlayer*` hooks as Oxide's covalence `OnUser*` hooks.

## Migrating

### Plugin developers (Carbon flavor)

- Derive from `CarbonPlugin` in `namespace Carbon.Plugins`. The familiar members are all there.
- Remove every `using Oxide.*;`. The types they brought in live in `Carbon.Plugins` (visible automatically inside `Carbon.Plugins`) and `Carbon.Components` (CUI types, `CuiHelper`).
- Replace Oxide-only APIs:

| Oxide API | Carbon replacement |
| --- | --- |
| `Interface.Oxide.DataFileSystem` | `Community.Runtime.DataFileSystem` |
| `Interface.CallHook` | `HookCaller.CallStaticHook(HookStringPool.GetOrAdd(name), …)` |
| `server.Command` | `ConsoleSystem.Run(ConsoleSystem.Option.Server, …)` |
| `plugins.Find` | `ModLoader.FindPlugin` |
| `IPlayer` / covalence | `BasePlayer`, plus `PlayerEx` helpers (`Rename`, `Ban`) |

- Only the base hooks (plugin lifecycle, server init/save/shutdown) and Carbon's Base + Community hooks exist. For anything else, use `[AutoPatch]` Harmony patches on nested classes.
- Precompiled plugins/extensions must be rebuilt against 3.0: namespaces and types moved.

### Plugin developers (Carbon with Oxide)

Nothing to do for regular Oxide plugins. CarbonPlugins that used Oxide types keep compiling through the translator. For new code, prefer the native API.

### Server owners

- Pick the archive flavor: `Carbon.<OS>.<Config>.zip`, or `Carbon.<OS>.<Config>.Oxide.zip` for Oxide plugins.
- Permission data moves from `oxide.users/groups.data` to `carbon.users/groups.data`. Carbon imports the old files once on first start, and leaves them in place.
- Admin panel: the uMod plugin browser only exists in the Oxide flavor. Its toggle is now "Disable external vendor".
- The Carbon flavor doesn't clean up a previous Oxide install. Verify your game files if you're coming from Oxide, or use the Oxide flavor, which does.

### Contributors

- `Community` is a single class (`CommunityInternal` is gone). `Carbon.dll` only contains `Initializer`.
- `BaseProcessor`/`IScriptProcessor`/… are gone. Use `Community.Runtime.PluginSources` (`Find`, `Prepare`, `Retry`, `LoadAll`, per-source `Entries`/`IgnoreList`).
- `Community.Runtime.ModuleProcessor.Modules` → `Community.Runtime.Modules.All`. `CarbonProcessor` next-frame queue → `Community.Runtime.Scheduler.NextFrame`.
- `API.Events`/`API.Commands`/`API.Hooks`/`API.Logger` → `Carbon.Events`/`Carbon.Commands`/`Carbon.Hooks`/`Carbon.Logging`. Addon contracts (`ICarbonComponent`, …) are in `Carbon`.
- Community hooks are built from source with every build. The CDN copies were compiled against 2.x.

## Known follow-ups

- `Carbon.Hooks.Oxide.dll` needs to be regenerated with the 3.0 generator (HOOKGEN pipeline). The CDN copy targets 2.x (`API.Hooks.Patch`) and won't load.
- The hook self-updater now downloads from `cdn.carbonmod.gg/hooks/server/3/...` (`Hooks.Updater.Channel`). Until the CDN serves that channel, updates fail gracefully and the hooks shipped with the build are kept.
- The docs site's "Oxide compatible" hook badge came from `MetadataAttribute.OxideCompatible`, which was removed from the SDK.
