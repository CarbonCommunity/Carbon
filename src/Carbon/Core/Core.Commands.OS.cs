using Application = UnityEngine.Application;

namespace Carbon.Core;

public partial class CorePlugin
{
	[ConsoleCommand("openplugin", "Locally opens the `cs` file of a loaded plugin.")]
	[AuthLevel(2)]
	private void OpenPlugin(ConsoleSystem.Arg arg)
	{
		var plugin = ModLoader.FindPlugin(arg.GetString(0));

		if (plugin == null)
		{
			arg.ReplyWith($"Couldn't find plugin.");
			return;
		}

		Application.OpenURL(plugin.FilePath);
		arg.ReplyWith($"Opened '{plugin.ToPrettyString()}' plugin file");
	}

	[ConsoleCommand("openroot", "Locally opens the root folder of Carbon.")]
	[AuthLevel(2)]
	private void OpenRoot(ConsoleSystem.Arg arg)
	{
		var folder = Defines.GetRootFolder();
		Application.OpenURL(folder);
		arg.ReplyWith($"Opened '{folder}'");
	}

	[ConsoleCommand("openconfigs", "Locally opens the configs folder of Carbon.")]
	[AuthLevel(2)]
	private void OpenConfigs(ConsoleSystem.Arg arg)
	{
		var folder = Defines.GetConfigsFolder();
		Application.OpenURL(folder);
		arg.ReplyWith($"Opened '{folder}'");
	}

	[ConsoleCommand("openmodules", "Locally opens the modules folder of Carbon.")]
	[AuthLevel(2)]
	private void OpenModules(ConsoleSystem.Arg arg)
	{
		var folder = Defines.GetModulesFolder();
		Application.OpenURL(folder);
		arg.ReplyWith($"Opened '{folder}'");
	}

	[ConsoleCommand("opendata", "Locally opens the data folder of Carbon.")]
	[AuthLevel(2)]
	private void OpenData(ConsoleSystem.Arg arg)
	{
		var folder = Defines.GetDataFolder();
		Application.OpenURL(folder);
		arg.ReplyWith($"Opened '{folder}'");
	}

	[ConsoleCommand("openplugins", "Locally opens the plugins folder of Carbon.")]
	[AuthLevel(2)]
	private void OpenPlugins(ConsoleSystem.Arg arg)
	{
		var folder = Defines.GetScriptsFolder();
		Application.OpenURL(folder);
		arg.ReplyWith($"Opened '{folder}'");
	}

	[ConsoleCommand("openextensions", "Locally opens the extensions folder of Carbon.")]
	[AuthLevel(2)]
	private void OpenExtensions(ConsoleSystem.Arg arg)
	{
		var folder = Defines.GetExtensionsFolder();
		Application.OpenURL(folder);
		arg.ReplyWith($"Opened '{folder}'");
	}

	[ConsoleCommand("openlogs", "Locally opens the logs folder of Carbon.")]
	[AuthLevel(2)]
	private void OpenLogs(ConsoleSystem.Arg arg)
	{
		var folder = Defines.GetLogsFolder();
		Application.OpenURL(folder);
		arg.ReplyWith($"Opened '{folder}'");
	}

	[ConsoleCommand("openlang", "Locally opens the language folder of Carbon.")]
	[AuthLevel(2)]
	private void OpenLang(ConsoleSystem.Arg arg)
	{
		var folder = Defines.GetLangFolder();
		Application.OpenURL(folder);
		arg.ReplyWith($"Opened '{folder}'");
	}

	[ConsoleCommand("delete", "Locally deletes a file or directory relative to the server root. . Syntax: c.deleteext \"path/to\" \"cs\"")]
	[AuthLevel(2)]
	private void Delete(ConsoleSystem.Arg arg)
	{
		var path = Path.Combine(Defines.GetRootFolder(), arg.GetString(0));
		OsEx.File.Delete(path);
		OsEx.Folder.Delete(path);
		arg.ReplyWith($"Deleted '{path}'");
	}

	[ConsoleCommand("deleteext", "Locally deletes all files with a specified extension relative to the server root. Syntax: c.deleteext \"path/to\" \"cs\"")]
	[AuthLevel(2)]
	private void DeleteExt(ConsoleSystem.Arg arg)
	{
		var folder = Path.Combine(Defines.GetRootFolder(), arg.GetString(0));
		var extension = arg.GetString(1);

		var files = OsEx.Folder.GetFilesWithExtension(folder, extension);
		OsEx.Folder.DeleteFilesWithExtension(folder, extension);
		for (int i = 0; i < files.Length; i++)
		{
			arg.ReplyWith($"Deleted '{files[i]}'");
		}
	}
}
