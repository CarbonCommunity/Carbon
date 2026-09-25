// Read by Carbon.Startup before Rust loads, see Carbon.StartupTaskAttribute / Carbon.InjectFieldAttribute

// Oxide plugins access the covalence player straight off BasePlayer
[assembly: InjectField("BasePlayer", "IPlayer", typeof(Oxide.Core.Libraries.Covalence.IPlayer))]

// Remove a previous Oxide install, it can't run next to Carbon
[assembly: StartupTask(StartupAction.Delete, "{rust_managed}/Oxide.Common.dll")]
[assembly: StartupTask(StartupAction.Delete, "{rust_managed}/Oxide.Core.dll")]
[assembly: StartupTask(StartupAction.Delete, "{rust_managed}/Oxide.CSharp.dll")]
[assembly: StartupTask(StartupAction.Delete, "{rust_managed}/Oxide.MySql.dll")]
[assembly: StartupTask(StartupAction.Delete, "{rust_managed}/Oxide.References.dll")]
[assembly: StartupTask(StartupAction.Delete, "{rust_managed}/Oxide.Rust.dll")]
[assembly: StartupTask(StartupAction.Delete, "{rust_managed}/Oxide.SQLite.dll")]
[assembly: StartupTask(StartupAction.Delete, "{rust_managed}/Oxide.Unity.dll")]

// Oxide extensions become Carbon extensions
[assembly: StartupTask(StartupAction.MoveMatching, "{rust_managed}", Target = "{extensions}", Filter = "Oxide.Ext.")]

// Fresh installs pick up the existing Oxide folders
[assembly: StartupTask(StartupAction.CopyIfEmpty, "{rust}/oxide/config", Target = "{carbon}/configs")]
[assembly: StartupTask(StartupAction.CopyIfEmpty, "{rust}/oxide/data", Target = "{carbon}/data")]
[assembly: StartupTask(StartupAction.CopyIfEmpty, "{rust}/oxide/plugins", Target = "{carbon}/plugins")]
[assembly: StartupTask(StartupAction.CopyIfEmpty, "{rust}/oxide/lang", Target = "{carbon}/lang")]
