namespace Carbon.Core;

public partial class CorePlugin
{
	[ConsoleCommand("addconditional", "Adds a new conditional compilation symbol to the compiler.")]
	[AuthLevel(2)]
	private void AddConditional(ConsoleSystem.Arg arg)
	{
		var value = arg.GetString(0);

		if (!Community.Runtime.Config.Compiler.ConditionalCompilationSymbols.Contains(value))
		{
			Community.Runtime.Config.Compiler.ConditionalCompilationSymbols.Add(value);
			Community.Runtime.SaveConfig();
			arg.ReplyWith($"Added conditional '{value}'.");
		}
		else
		{
			arg.ReplyWith($"Conditional '{value}' already exists.");
		}

		for(int i = 0; i < ModLoader.Packages.Count; i++)
		{
			var package = ModLoader.Packages[i];
			using var plugins = Facepunch.Pool.Get<PooledList<RustPlugin>>();
			plugins.AddRange(package.Plugins);
			for(int p = 0; p < plugins.Count; p++)
			{
				var plugin = plugins[p];
				if (!plugin.HasConditionals)
				{
					continue;
				}
				plugin.ProcessorProcess.Dispose();
				plugin.ProcessorProcess.Execute(plugin.Processor);
				package.Plugins.Remove(plugin);
			}
		}
	}

	[ConsoleCommand("remconditional", "Removes an existent conditional compilation symbol from the compiler.")]
	[AuthLevel(2)]
	private void RemoveConditional(ConsoleSystem.Arg arg)
	{
		var value = arg.GetString(0);

		if (Community.Runtime.Config.Compiler.ConditionalCompilationSymbols.Contains(value))
		{
			Community.Runtime.Config.Compiler.ConditionalCompilationSymbols.Remove(value);
			Community.Runtime.SaveConfig();
			arg.ReplyWith($"Removed conditional '{value}'.");
		}
		else
		{
			arg.ReplyWith($"Conditional '{value}' does not exist.");
		}

		for(int i = 0; i < ModLoader.Packages.Count; i++)
		{
			var package = ModLoader.Packages[i];
			using var plugins = Facepunch.Pool.Get<PooledList<RustPlugin>>();
			plugins.AddRange(package.Plugins);
			for(int p = 0; p < plugins.Count; p++)
			{
				var plugin = plugins[p];
				if (!plugin.HasConditionals)
				{
					continue;
				}
				plugin.ProcessorProcess.Dispose();
				plugin.ProcessorProcess.Execute(plugin.Processor);
				package.Plugins.Remove(plugin);
			}
		}
	}

	[ConsoleCommand("conditionals", "Prints a list of all conditional compilation symbols used by the compiler.")]
	[AuthLevel(2)]
	private void Conditionals(ConsoleSystem.Arg arg)
	{
		arg.ReplyWith($"Conditionals ({Community.Runtime.Config.Compiler.ConditionalCompilationSymbols.Count:n0}): {Community.Runtime.Config.Compiler.ConditionalCompilationSymbols.ToString(", ", " and ")}");
	}
}
