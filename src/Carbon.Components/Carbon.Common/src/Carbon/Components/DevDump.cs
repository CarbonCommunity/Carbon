using System.IO.Compression;
using System.Text;
using Facepunch;
using Network;

namespace Carbon.Components;

public class DevDump : Pool.IPooled
{
	public bool includeServerLog;
	public MonoProfiler.Sample sample;

	public void Init(bool includeServerLog)
	{
		this.includeServerLog = includeServerLog;
	}

	public void Export(float duration, string path, Action onComplete = null)
	{
		MonoProfiler.ToggleProfilingTimed(duration, MonoProfiler.AllFlags, args =>
		{
			sample.Resample();
			Export(path);
			onComplete?.Invoke();
		});
	}

	public void Export(string path)
	{
		using var zipToOpen = new FileStream(path, FileMode.Create);
		using var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create);

		if (includeServerLog)
		{
			var logFile = CommandLineEx.GetArgumentResult("-logFile") ??  CommandLineEx.GetArgumentResult("-logfile");
			if (!string.IsNullOrEmpty(logFile) && OsEx.File.Exists(logFile))
			{
				using (FileStream fs = new FileStream(logFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				{
					var buffer = BaseNetwork.ArrayPool.Rent((int)fs.Length * 2);
					fs.Read(buffer, 0, (int)fs.Length);
					AddFile(archive, "output.log", buffer);
					BaseNetwork.ArrayPool.Return(buffer);
				}

			}
			else if(!string.IsNullOrEmpty(logFile))
			{
				Logger.Log($"Couldn't find log file '{Path.GetFullPath(logFile)}'");
			}
		}

		if (!sample.IsCleared)
		{
			AddFile(archive, $"profile.{MonoProfiler.ProfileExtension}", sample.ToProto());
			AddFile(archive, "profile.csv", Encoding.UTF8.GetBytes(sample.ToCSV()));
		}
	}

	private static void AddFile(ZipArchive archive, string fileName, byte[] content)
	{
		var entry = archive.CreateEntry(fileName);
		using Stream entryStream = entry.Open();
		entryStream.Write(content, 0, content.Length);
	}

	public void EnterPool()
	{
		includeServerLog = false;
		sample.Clear();
	}

	public void LeavePool()
	{
	}
}
