namespace Carbon.Core;

public partial class CorePlugin
{
#if DEBUG
	[CommandVar("scriptdebugorigin", "[For debugging purposes] Overrides the script directory to this value so remote debugging is possible.")]
	[AuthLevel(2)]
	private string ScriptDebuggingOrigin { get { return Community.Runtime.Config.Debugging.ScriptDebuggingOrigin; } set { Community.Runtime.Config.Debugging.ScriptDebuggingOrigin = value; } }
#endif

	[CommandVar("hooklsthreshold", "The threshold value used by the hook caller to determine what minimum time is considered as a server lag spike. Defaults to 1000ms.")]
	[AuthLevel(2)]
	private int HookLagSpikeThreshold { get { return Community.Runtime.Config.Debugging.HookLagSpikeThreshold; } set { Community.Runtime.Config.Debugging.HookLagSpikeThreshold = value.Clamp(100, 10000); } }

	[ConsoleCommand("resethooks", "Clears all progress on all of the current hooks (hook time, fires, memory usage, exceptions and lag spikes).")]
	[AuthLevel(2)]
	private void ResetHooks(ConsoleSystem.Arg arg)
	{
		foreach (var plugin in ModLoader.Packages.SelectMany(package => package.Plugins))
		{
			plugin.HookPool.Reset();
		}

		foreach (var module in Community.Runtime.ModuleProcessor.Modules)
		{
			module.HookPool.Reset();
		}

		arg.ReplyWith($"All plugin and module hook cache has been reset.");
	}

	[ConsoleCommand("printhookpool", "Print currently allocated hook argument pool memory")]
	[AuthLevel(2)]
	private void PrintHookPool(ConsoleSystem.Arg arg)
	{
		using var table = new StringTable("arg_count", "rented", "rented_extra", "returned", "stack_count", "max_size");
		foreach(var argument in HookCaller.Caller._argumentBuffer)
		{
			var value = argument.Value;
			table.AddRow(argument.Key, value.Rented, value.RentedExtra, value.Returned, value.Count, HookCallerCommon.HookArgPool.BufferSize);
		}
		arg.ReplyWith(table.Write(StringTable.FormatTypes.None));
	}
}
