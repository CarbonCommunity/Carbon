using Application = UnityEngine.Application;

namespace Carbon.Core;

public partial class CorePlugin
{
#if WIN

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

#endif

	[ConsoleCommand("delete", "Locally deletes a file or directory relative to the server root. Syntax: c.deleteext \"path/to\"")]
	[AuthLevel(2)]
	private void Delete(ConsoleSystem.Arg arg)
	{
		if (!arg.HasArgs())
		{
			arg.ReplyWith($"No arguments provided!\n" +
			              $"Syntax: c.deleteext \"path/to\"\n" +
			              $"Syntax: c.deleteext \"path/to/file.txt\"");
			return;
		}

		var path = Path.Combine(Defines.GetRootFolder(), arg.GetString(0));
		if (OsEx.File.Exists(path) || OsEx.Folder.Exists(path))
		{
			OsEx.File.Delete(path);
			OsEx.Folder.Delete(path);
			arg.ReplyWith($"Deleted '{path}'");
		}
		else
		{
			arg.ReplyWith($"Couldn't delete '{path}' as it doesn't exist.");
		}
	}

	[ConsoleCommand("deleteext", "Locally deletes all files with a specified extension relative to the server root. Syntax: c.deleteext \"path/to\" \"cs\"")]
	[AuthLevel(2)]
	private void DeleteExt(ConsoleSystem.Arg arg)
	{
		if (!arg.HasArgs(2))
		{
			arg.ReplyWith($"Not enough arguments provided!\n" +
			              $"Syntax: c.deleteext \"path/to\" \"cs\"");
			return;
		}

		var folder = Path.Combine(Defines.GetRootFolder(), arg.GetString(0));
		var extension = arg.GetString(1);
		var files = OsEx.Folder.GetFilesWithExtension(folder, extension);
		OsEx.Folder.DeleteFilesWithExtension(folder, extension);
		for (int i = 0; i < files.Length; i++)
		{
			arg.ReplyWith($"Deleted '{files[i]}'");
		}
	}

	[ConsoleCommand("createplugin", "It creates a new plugin in the plugins folder. Syntax: c.createplugin \"PluginName\" \"Author\" \"Description\"")]
	[AuthLevel(2)]
	private void CreatePlugin(ConsoleSystem.Arg arg)
	{
		var name = arg.GetString(0, "NewPlugin");
		var sanitizedName = name.Replace(" ", string.Empty);
		var fileName = Path.Combine(Defines.GetScriptsFolder(), sanitizedName + ".cs");

		if (OsEx.File.Exists(fileName))
		{
			arg.ReplyWith("A plugin with the same name already exists.");
			return;
		}

		var author = arg.GetString(1, Environment.UserName);
		var description = arg.GetString(2, "New cool plugin that does things!");
		OsEx.File.Create(fileName,
			content: $"namespace Carbon.Plugins;\n\n" +
			         $"[Info(\"{name}\", \"{author}\", \"1.0\")]\n" +
			         $"[Description(\"{description}\")]\n" +
			         $"public class {sanitizedName} : CarbonPlugin\n" +
			         $"{{\n" +
			         $"\tprivate void OnServerInitialized()\n" +
			         $"\t{{\n" +
			         $"\t\tPuts(\"New plugin is here!\");\n" +
			         $"\t}}\n" +
			         $"}}");
	}
}
