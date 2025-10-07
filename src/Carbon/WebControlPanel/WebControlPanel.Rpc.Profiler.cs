using Facepunch.Math;

namespace Carbon;

public static partial class WebControlPanel
{
	[WebCall]
	[WebCall.Condition.Permission(PermissionTypes.ProfilerView)]
	private static void RPC_ProfilesList(BridgeRead read)
	{
		var write = StartRpcResponse();
		var files = Directory.GetFiles(Defines.GetProfilesFolder(), $"*.{MonoProfiler.ProfileExtension}").OrderByDescending(x => new FileInfo(x).LastWriteTime);
		write.WriteObject(files.Count());
		foreach(var file in files)
		{
			var info = new FileInfo(file);
			write.WriteObject(file);
			write.WriteObject(Path.GetFileNameWithoutExtension(file));
			write.WriteObject(info.Length);
			write.WriteObject(Epoch.FromDateTime(info.LastWriteTimeUtc));
		}
		SendRpcResponse(read.Connection, write);
	}

	[WebCall]
	[WebCall.Condition.Permission(PermissionTypes.ProfilerLoad)]
	private static void RPC_ProfilesLoad(BridgeRead read)
	{
		var path = Path.GetFullPath(read.String());

		if (!File.Exists(path) || Path.GetDirectoryName(path) != Defines.GetProfilesFolder())
		{
			return;
		}

		var profile = File.ReadAllBytes(path);
		var write = StartRpcResponse();
		write.WriteObject(Path.GetFileName(path));
		write.WriteObject(profile.Length);
		write.WriteObject(profile);
		SendRpcResponse(read.Connection, write);
	}

	[WebCall]
	[WebCall.Condition.Permission(PermissionTypes.ProfilerEdit)]
	private static void RPC_ProfilesDelete(BridgeRead read)
	{
		var path = Path.GetFullPath(read.String());

		if (!File.Exists(path) || Path.GetDirectoryName(path) != Defines.GetProfilesFolder())
		{
			return;
		}

		File.Delete(path);
	}

	[WebCall]
	[WebCall.Condition.Permission(PermissionTypes.ProfilerView)]
	private static void RPC_ProfilesState(BridgeRead read)
	{
		var write = StartRpcResponse();
		write.WriteObject(MonoProfiler.IsRecording);
		write.WriteObject(MonoProfiler.Enabled);
		write.WriteObject(MonoProfiler.Crashed);
		write.WriteObject((float)MonoProfiler.CurrentDurationTime.TotalSeconds);
		SendRpcResponse(read.Connection, write);
	}

	[WebCall]
	[WebCall.Condition.Permission(PermissionTypes.ProfilerEdit)]
	private static void RPC_ProfilesToggle(BridgeRead read)
	{
		var wantsCancel = read.Bool();
		var flags = MonoProfiler.ProfilerArgs.None;

		var callMemory = read.Bool();
		var advancedMemory = read.Bool();
		var timings = read.Bool();
		var calls = read.Bool();
		var gcEvents = read.Bool();
		var stackWalkAllocsEvents = read.Bool();

		if (callMemory) flags |= MonoProfiler.ProfilerArgs.CallMemory;
		if (advancedMemory) flags |= MonoProfiler.ProfilerArgs.AdvancedMemory;
		if (timings) flags |= MonoProfiler.ProfilerArgs.Timings;
		if (calls) flags |= MonoProfiler.ProfilerArgs.Calls;
		if (gcEvents) flags |= MonoProfiler.ProfilerArgs.GCEvents;
		if (stackWalkAllocsEvents) flags |= MonoProfiler.ProfilerArgs.StackWalkAllocations;

		if (flags == MonoProfiler.ProfilerArgs.None) flags = MonoProfiler.AllFlags;

		Community.Runtime.Core.NextFrame(() =>
		{
			if (MonoProfiler.IsRecording)
			{
				if (wantsCancel)
				{
					MonoProfiler.ToggleProfiling(MonoProfiler.ProfilerArgs.Abort, logging: false);
				}
				else
				{
					var sample = MonoProfiler.Sample.Create();
					MonoProfiler.ToggleProfiling(logging: false);
					sample.Resample();
					var date = DateTime.Now;
					var file = Path.Combine(Defines.GetProfilesFolder(), $"profile-{date.Year}_{date.Month}_{date.Day}_{date.Hour}{date.Minute}{date.Second}.{MonoProfiler.ProfileExtension}");
					File.WriteAllBytes(file, sample.ToProto());
					MonoProfiler.Clear();
				}
			}
			else
			{
				MonoProfiler.ToggleProfiling(flags, logging: false);
			}
		});
	}
}
