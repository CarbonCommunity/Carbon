using Facepunch;

namespace Carbon;

public static partial class WebControlPanel
{
	[WebCall]
	[WebCall.Condition.Permission(PermissionTypes.PluginsView)]
	private static void RPC_Plugins(BridgeRead read)
	{
		var write = StartRpcResponse();
		using var plugins = Pool.Get<PooledList<RustPlugin>>();
		ModLoader.Packages.GetAllHookables(plugins, true);
		write.WriteObject(plugins.Count);
		for (int i = 0; i < plugins.Count; i++)
		{
			new PluginInfo(plugins[i]).Serialize(write);
		}

		using var unloadedPlugins = Pool.Get<PooledList<string>>();
		unloadedPlugins.AddRange(Community.Runtime.ScriptProcessor.IgnoreList);
		unloadedPlugins.AddRange(Community.Runtime.ZipScriptProcessor.IgnoreList);
#if DEBUG
		unloadedPlugins.AddRange(Community.Runtime.ZipDevScriptProcessor.IgnoreList);
#endif
		write.WriteObject(unloadedPlugins.Count);
		for (int i = 0; i < unloadedPlugins.Count; i++)
		{
			write.WriteObject(Path.GetFileNameWithoutExtension(unloadedPlugins[i]));
		}

		using var failedCompilations = Pool.Get<PooledList<ModLoader.CompilationResult>>();
		foreach (var compilation in ModLoader.FailedCompilations.Values)
		{
			if (!compilation.HasFailed())
			{
				continue;
			}
			failedCompilations.Add(compilation);
		}
		write.WriteObject(failedCompilations.Count);
		for(int i = 0; i < failedCompilations.Count; i++)
		{
			var failedPlugin = failedCompilations[i];
			write.WriteObject(Path.GetFileNameWithoutExtension(failedPlugin.File));
			write.WriteObject(failedPlugin.Errors.Count);
			for (int e = 0; e < failedPlugin.Errors.Count; e++)
			{
				var error = failedPlugin.Errors[e];
				write.WriteObject(error.Message);
				write.WriteObject(error.Number);
				write.WriteObject(error.Column);
				write.WriteObject(error.Line);
			}
		}

		SendRpcResponse(read.Connection, write);
	}

	[WebCall]
	[WebCall.Condition.Permission(PermissionTypes.PluginsEdit)]
	private static void RPC_PluginsUnload(BridgeRead read)
	{
		var fileName = Path.GetFileNameWithoutExtension(read.String());
		var plugin = ModLoader.FindPlugin(fileName);
		if (plugin == null)
		{
			return;
		}
		CorePlugin.ProcessableFilesLookup();
		var path = CorePlugin.GetPluginFile(fileName);
		if (!string.IsNullOrEmpty(path.Path))
		{
			path.GetProcessor().Ignore(path.Path);
		}
		ModLoader.UninitializePlugin(plugin);
	}

	[WebCall]
	[WebCall.Condition.Permission(PermissionTypes.PluginsEdit)]
	private static void RPC_PluginsLoad(BridgeRead read)
	{
		var fileName = read.String();
		CorePlugin.ProcessableFilesLookup();
		var path = CorePlugin.GetPluginFile(fileName);
		if (!string.IsNullOrEmpty(path.Path) && path.GetProcessor() is IBaseProcessor processor)
		{
			processor.ClearIgnore(path.Path);
			processor.Prepare(path.Id, path.Path);
		}
	}

	public struct PluginInfo(RustPlugin plugin)
	{
		private string name = plugin.Name;
		private string fileName = plugin.FileName;
		private string version = plugin.Version.ToString();
		private string author = plugin.Author;
		private string description = plugin.Description;

		public void Serialize(BridgeWrite write)
		{
			write.WriteObject(name);
			write.WriteObject(fileName);
			write.WriteObject(version);
			write.WriteObject(author);
			write.WriteObject(description);
		}
	}
}
