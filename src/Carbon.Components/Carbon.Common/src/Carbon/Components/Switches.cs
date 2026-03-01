namespace Carbon;

public sealed class Switches
{
	[Switch(Name = "+carbon.onboot", Help = "Executes a set of commands separated by \"|\" that get executed when Carbon boots. Can be defined in \"cfg/server.cfg\" as well.")]
	public static string GetOnBootCommands(string defaultValue = null) => CommandLineEx.GetArgumentResult("+carbon.onboot", defaultValue);

	[Switch(Name = "+carbon.onserverinit", Help = "Executes a set of commands separated by \"|\" that get executed when the server becomes fully initialized. Can be defined in \"cfg/server.cfg\" as well.")]
	public static string GetOnServerInitCommands(string defaultValue = null) => CommandLineEx.GetArgumentResult("+carbon.onserverinit", defaultValue);

	[Switch(Name = "-carbon.rootdir", Help = "Overrides the root directory of Carbon, aka the \"carbon\" folder. Upon changing this, you must also apply another command line variable which links to the custom Carbon root, Carbon.Preloader.dll;\n--doorstop-target-assembly \"customrootdir/managed/Carbon.Preloader.dll\"")]
	public static string GetRootDir(string defaultValue = null) => CommandLineEx.GetArgumentResult("-carbon.rootdir", defaultValue);

	[Switch(Name = "-carbon.carbonconfigdir", Help = "Overrides the settings root directory where Carbon stores its configuration files")]
	public static string GetCarbonConfigDir(string defaultValue = null) => CommandLineEx.GetArgumentResult("-carbon.carbonconfigdir", defaultValue);

	[Switch(Name = "-carbon.scriptdir", Help = "Overrides the scripts directory of Carbon, aka the \"carbon/plugins\" folder.")]
	public static string GetScriptDir(string defaultValue = null) => CommandLineEx.GetArgumentResult("-carbon.scriptdir", defaultValue);

	[Switch(Name = "-carbon.configdir", Help = "Overrides the config directory of Carbon, aka the \"carbon/configs\" folder.")]
	public static string GetConfigDir(string defaultValue = null) => CommandLineEx.GetArgumentResult("-carbon.configdir", defaultValue);

	[Switch(Name = "-carbon.datadir", Help = "Overrides the data directory of Carbon, aka the \"carbon/data\" folder.")]
	public static string GetDataDir(string defaultValue = null) => CommandLineEx.GetArgumentResult("-carbon.datadir", defaultValue);

	[Switch(Name = "-carbon.modifierdir", Help = "Overrides the modifier directory of Carbon, aka the \"carbon/modifiers\" folder.")]
	public static string GetModifierDir(string defaultValue = null) => CommandLineEx.GetArgumentResult("-carbon.modifierdir", defaultValue);

	[Switch(Name = "-carbon.langdir", Help = "Overrides the lang directory of Carbon, aka the \"carbon/lang\" folder.")]
	public static string GetLangDir(string defaultValue = null) => CommandLineEx.GetArgumentResult("-carbon.langdir", defaultValue);

	[Switch(Name = "-carbon.extdir", Help = "Overrides the extensions directory of Carbon, aka the \"carbon/extensions\" folder.")]
	public static string GetExtDir(string defaultValue = null) => CommandLineEx.GetArgumentResult("-carbon.extdir", defaultValue);

	[Switch(Name = "-carbon.moduledir", Help = "Overrides the modules directory of Carbon, aka the \"carbon/modules\" folder.")]
	public static string GetModuleDir(string defaultValue = null) => CommandLineEx.GetArgumentResult("-carbon.moduledir", defaultValue);

	[Switch(Name = "-carbon.logdir", Help = "Overrides the log directory of Carbon, aka the \"carbon/logs\" folder.")]
	public static string GetLogDir(string defaultValue = null) => CommandLineEx.GetArgumentResult("-carbon.logdir", defaultValue);

	[Switch(Name = "-carbon.profiledir", Help = "Overrides the mono profiler results directory of Carbon, aka the \"carbon/profiles\" folder.")]
	public static string GetProfileDir(string defaultValue = null) => CommandLineEx.GetArgumentResult("-carbon.profiledir", defaultValue);

	[Switch(Name = "-carbon.sqlpermsdb", Help = "Overrides the SQLite Permissions database path, aka the \"server/identity/carbon.perms.db\" file.")]
	public static string GetSQLPermissionsDatabase(string defaultValue = null) => CommandLineEx.GetArgumentResult("-carbon.sqlpermsdb", defaultValue);

	[Switch(Name = "-harmonydir", Help = "Overrides the directory of Rust's HarmonyMods folder.")]
	public static string GetHarmonyDir(string defaultValue = null) => CommandLineEx.GetArgumentResult("-harmonydir", defaultValue);
}

[AttributeUsage(AttributeTargets.Method)]
public class SwitchAttribute : Attribute
{
	public string Name { get; set; }
	public string Help { get; set; }
}
