# Copilot Instructions

Carbon is a self-updating, lightweight, Harmony-based mod loader for the game **Rust**,
written in modern C#. It is the spiritual successor to / replacement for Oxide and keeps
backward compatibility with Oxide plugins. It provides a permission system, user system,
in-game GUI, modules, and an automated hook-generation pipeline.

- **Repository:** https://github.com/CarbonCommunity/Carbon
- **License:** GNU GPL v3
- **Docs:** https://carbonmod.gg/ · **Hooks:** https://carbonmod.gg/references/hooks/ · **Discord:** https://discord.gg/carbonmod

## Tech stack

- **Language:** C# with `<LangVersion>preview</LangVersion>` (use the latest C# features).
- **Target framework:** `net48` (.NET Framework 4.8) — Carbon is injected into Rust's Mono/Unity runtime.
  Do **not** assume .NET Core/5+ BCL APIs are available; prefer APIs present in net48 + the project's polyfills.
- **Runtime:** Unity / Mono. `UnityEngine` is globally available in `Carbon.Common`.
- **Patching:** [Harmony](https://github.com/pardeike/Harmony) for hooks/patches.
- **Platform:** `x64` only.

## Repository layout

| Path | Purpose |
| --- | --- |
| `src/Carbon` | Main Carbon entry assembly, loaders, managers, processors, threads. |
| `src/Carbon.Components/Carbon.Preloader` | Runtime preloader of dependencies; handles self-updating. Invokes Carbon.Startup. |
| `src/Carbon.Components/Carbon.Startup` | In-memory Rust assembly patching, publicizing and exporting (Developer Mode). |
| `src/Carbon.Components/Carbon.Bootstrap` | Initial Carbon boot in the primary app-domain. |
| `src/Carbon.Components/Carbon.Common` | Core of Carbon — tools, extensions, base types. Centerpiece all other components depend on. |
| `src/Carbon.Components/Carbon.SDK` | Contracts/infrastructure with **no implementation** (interfaces, abstractions). |
| `src/Carbon.Components/Carbon.Modules` | Optional modules expanding functionality / QoL. |
| `src/Carbon.Components/Carbon.Compat` | Compatibility loader (Oxide extensions/plugins compatibility). |
| `src/Carbon.Components/Carbon.Test` | Automated testing rules and events infrastructure. |
| `src/Carbon.Hooks/Carbon.Hooks.Base` | Ground-level dynamic & static patches for Carbon's own runtime. |
| `src/Carbon.Hooks/Carbon.Hooks.Community` | Community-curated patches and hooks. |
| `src/Carbon.Hooks/Carbon.Hooks.Generator` | Generates Carbon Harmony hooks from an Oxide `.opj` file. |
| `src/Carbon.Hooks/Carbon.Hooks.Oxide` | Oxide compatibility package, used internally during patch code generation. |
| `src/Carbon.Tools` | Source generators, compiler polyfills, the publicizer, and the test runner. |
| `src/Carbon.Profiler*` | Mono profiler and its tests/harness. |
| `src/Carbon.Tests` | Automated end-to-end test runner suite (used in CI). |
| `src/.msbuild` | Shared MSBuild props/targets (`Common.props`, `Configurations.props`, etc.). |
| `tools/build/{win,linux}` | Build & update scripts invoked by `setup.bat`/`setup.sh`. |
| `tools/depot` | Submodule: DepotDownloader for fetching Rust server assemblies. |
| `src/Carbon.Native` | Submodule: native components. |
| `rust/` | Local Rust server reference assemblies (populated by setup). |
| `release/` | Build output. |

Carbon is split into mostly-independent **components** (`src/Carbon.Components`) and **hooks**
(`src/Carbon.Hooks`). The solution file is `src/Carbon.slnx`.

## Building & running

- **First-time setup / full build:** run `setup.bat` (Windows) or `setup.sh` (Linux) from the repo root.
  This bootstraps dependencies (including git submodules and Rust reference assemblies) and produces
  output under `./release`.
- This repository uses **git submodules** (`src/Carbon.Native`, `tools/depot`) — ensure they are
  initialized (`git submodule update --init --recursive`); setup handles this.
- **Build configurations:** `Debug`, `Release`, `Minimal`, each with a `*Unix` variant
  (e.g. `DebugUnix`). Default is `Debug`/`x64`.
  - `Unix` variants define `UNIX`; otherwise `WIN` is defined.
  - `Minimal` defines `MINIMAL` (trimmed build).
  - Use the corresponding scripts in `tools/build/win` / `tools/build/linux`
    (e.g. `build_debug.bat`, `build_release.sh`).
- **Conditional compilation:** guard platform/config-specific code with `#if WIN` / `#if UNIX`,
  `#if DEBUG`, and `#if MINIMAL` — these symbols are set per configuration.
- Release/Minimal builds set `TreatWarningsAsErrors=true`. Keep code warning-clean for those configs.

## Tests

- `src/Carbon.Tests` is the automated runner suite, primarily driven by CI
  (`.github/workflows/common-test.yml`). It downloads a Rust server (via DepotDownloader) and a Carbon
  build, then runs the test plugin. See `src/Carbon.Tests/README.md` for the required env variables
  (e.g. `WorkingDir`, `BranchName`, `CarbonDownloadZipUrl`) and the `Tests__No*` opt-out flags.
- `.run/` contains Rider/VS run configs (`Carbon.Tests (DEV+ENV)`, `Carbon.Profiler.Tests (DEV+ENV)`)
  that read a local `.env` file.
- There are also `Carbon.Profiler.Tests` for the profiler.

## Code style & conventions

Style is enforced via `src/.editorconfig` (`EnforceCodeStyleInBuild=true`). Key rules:

- **Indentation:** tabs (indent size 4). XML project/config files use indent size 2 (still tabs).
- **Line endings:** CRLF for most files; **LF for `*.sh`**. C# files use **UTF-8 with BOM**.
- **Line length:** soft guides at columns 80 and 130; `max_line_length` 160.
- **`using` directives:** System namespaces sorted first; imports organized on format.
  Many common namespaces are already declared as `global using` in `GlobalUsings.cs` per project
  (e.g. `System`, `System.Linq`, `Carbon.*`, `Oxide.*`, `UnityEngine` in `Carbon.Common`) —
  don't re-import those; add new shared imports there when broadly used.
- **Namespaces:** use **file-scoped namespaces** (`namespace Carbon;`).
- **Braces:** Allman style — open brace on a new line (`csharp_new_line_before_open_brace = all`),
  including before `else`/`catch`/`finally`.
- **`var`:** preferred everywhere (built-in types, when apparent, and elsewhere).
- **Members:** expression-bodied **properties/indexers/accessors** are fine; expression-bodied
  **methods/constructors/operators** are not preferred (use block bodies).
- **Accessibility modifiers:** required for non-interface members.
- Prefer pattern matching, null-propagation, null-coalescing, object/collection initializers,
  and `readonly` fields where applicable.
- **Constants:** PascalCase. General .NET naming conventions otherwise.
- Modifier order: `public, private, protected, internal, static, extern, new, virtual, abstract,
  sealed, override, readonly, unsafe, volatile, async`.

### File headers

Files contributed by community members / carried over from other projects carry a GPL v3 copyright
header comment block at the top (e.g. `Copyright (c) <year> <author>, under the GNU v3 license rights`).
Preserve existing headers when editing; match the surrounding file when adding new ones.

## Working in this codebase

- **Match the surrounding code.** This is a large, established codebase — mirror local naming,
  patterns, and structure rather than introducing new idioms.
- Carbon mirrors / extends the **Oxide** API surface for plugin compatibility. When touching
  plugin-facing or Oxide-compat code, preserve backward compatibility.
- **Hooks** are largely **generated** (`Carbon.Hooks.Generator` from Oxide `.opj` files). Prefer
  understanding the generation pipeline over hand-editing generated hook output.
- The `Carbon.SDK` project is contracts-only — put interfaces/abstractions there, implementations
  in the appropriate component.
- Some Rust types are made accessible via the **publicizer** (`Carbon.Tools/Carbon.Publicizer`) and
  source generators (`Carbon.Tools/Carbon.SourceGenerators`). Compiler polyfills
  (`Carbon.CompilerPolyfills.*`) backfill newer language constructs onto net48.
- `InternalsVisibleTo` is used between projects (e.g. `Carbon.Common` → `Carbon.Hooks.Base`).

## Branches

Carbon tracks specific Rust release channels. Open PRs against the branch matching the target channel:

- `main` — primary base (Rust `public`); built as the edge build, updated on every commit.
- `experimental` — experimental features, usually synced with `main`.
- `production` — live release branch (Rust `public`/`release` on wipe day).
- `rust_beta/staging`, `rust_beta/aux01`, `rust_beta/aux02` — track Rust `staging`/`aux*` beta branches.

When unsure, target `main`.

## CI

GitHub Actions live in `.github/workflows`. Pull requests trigger `pull-request.yml`, which runs a
build (`common-build.yml`) followed by tests (`common-test.yml`). Markdown-only changes are ignored.
Keep builds warning-clean (warnings are errors in Release/Minimal) and ensure tests pass.
