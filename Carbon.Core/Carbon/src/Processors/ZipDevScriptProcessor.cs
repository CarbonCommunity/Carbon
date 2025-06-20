#if DEBUG
using System;
using System.Collections;
using System.IO;
using Carbon.Base;
using Carbon.Components;
using Carbon.Contracts;
using Carbon.Core;
using Carbon.Extensions;

namespace Carbon.Managers;

public class ZipDevScriptProcessor : BaseProcessor, IZipDevScriptProcessor
{
	public override string Name => "ZipDebugScript Processor";
	public override bool EnableWatcher => !Community.IsConfigReady || Community.Runtime.Config.Watchers.ZipScriptWatchers;
	public override string Folder => Defines.GetZipDevFolder();
	public override string Extension => ".cs";
	public override float Rate => Community.Runtime.Config.Processors.ZipScriptProcessingRate;
	public override Type IndexedType => typeof(ZipDevScript);

	private static readonly char[] DirectorySeparators = [Path.DirectorySeparatorChar];

	public override void Start()
	{
		BlacklistPattern =
		[
			"backups",
			"debug"
		];

		base.Start();

		IncludeSubdirectories = true;
	}

	public bool AllPendingScriptsComplete()
	{
		foreach (var instance in InstanceBuffer)
		{
			if (instance.Value is ZipDevScript script)
			{
				if (script.Loader != null && !script.Loader.HasFinished) return false;
			}
		}

		return true;
	}
	public bool AllNonRequiresScriptsComplete()
	{
		foreach (var instance in InstanceBuffer)
		{
			if (instance.Value is ZipDevScript script)
			{
				if (script.Loader != null && !script.Loader.HasRequires && !script.Loader.HasFinished) return false;
			}
		}

		return true;
	}
	public bool AllExtensionsComplete()
	{
		foreach (var instance in InstanceBuffer)
		{
			if (instance.Value is ZipDevScript script)
			{
				if (script.Loader != null && !script.Loader.IsExtension && !script.Loader.HasFinished) return false;
			}
		}

		return true;
	}

	void IScriptProcessor.StartCoroutine(IEnumerator coroutine)
	{
		StartCoroutine(coroutine);
	}
	void IScriptProcessor.StopCoroutine(IEnumerator coroutine)
	{
		StopCoroutine(coroutine);
	}

	public string GetZipScriptName(string source)
	{
		var cszipDevDir = Defines.GetZipDevFolder();
		return Path.Combine(cszipDevDir, source.Replace(cszipDevDir, string.Empty).Split(DirectorySeparators, StringSplitOptions.RemoveEmptyEntries)[0]);
	}

	public override void OnCreated(object sender, FileSystemEventArgs e)
	{
		if (!EnableWatcher || IsBlacklisted(e.FullPath)) return;

		var directory = GetZipScriptName(e.FullPath);

		if (InstanceBuffer.TryGetValue(directory, out var instance1))
		{
			instance1?.MarkDirty();
			return;
		}

		InstanceBuffer.Add(directory, null);
	}
	public override void OnChanged(object sender, FileSystemEventArgs e)
	{
		if (!EnableWatcher || IsBlacklisted(e.FullPath)) return;

		var directory = GetZipScriptName(e.FullPath);

		if (InstanceBuffer.TryGetValue(directory, out var mod))
		{
			mod.MarkDirty();
		}
	}
	public override void OnRenamed(object sender, RenamedEventArgs e)
	{
		if (!EnableWatcher || IsBlacklisted(e.FullPath)) return;

		var directory = GetZipScriptName(e.FullPath);

		if (InstanceBuffer.TryGetValue(directory, out var mod))
		{
			mod.MarkDeleted();
		}
		InstanceBuffer.Add(directory, null);
	}
	public override void OnRemoved(object sender, FileSystemEventArgs e)
	{
		if (!EnableWatcher || IsBlacklisted(e.FullPath)) return;

		var directory = GetZipScriptName(e.FullPath);

		if (InstanceBuffer.TryGetValue(directory, out var mod))
		{
			mod.MarkDeleted();
		}
	}

	public class ZipDevScript : Process, IScriptProcessor.IScript
	{
		public IScriptLoader Loader { get; set; }

		public override IBaseProcessor.IParser Parser => new ZipDevScriptParser();

		public override void Clear()
		{
			try
			{
				Loader?.Clear();
			}
			catch (Exception ex)
			{
				Logger.Error($"Error clearing {File}", ex);
			}
		}
		public override void Dispose()
		{
			try
			{
				Loader?.Clear();
			}
			catch (Exception ex)
			{
				Logger.Error($"Error disposing {File}", ex);
			}
		}
		public override void Execute(IBaseProcessor processor)
		{
			base.Execute(processor);

			try
			{
				ModLoader.GetCompilationResult(File).Clear();

				if (!OsEx.Folder.Exists(File))
				{
					Dispose();
					return;
				}

				Loader = new ScriptLoader
				{
					Parser = Parser,
					Mod = Community.Runtime.ZipPlugins,
					Process = this,
					BypassFileNameChecks = true
				};

				foreach (var file in OsEx.Folder.GetFilesWithExtension(File, processor.Extension))
				{
					var content = OsEx.File.ReadText(file);

					Loader.Sources.Add(new BaseSource
					{
						ContextFilePath = File,
						ContextFileName = Path.GetFileName(File),
						FilePath = file,
						FileName = Path.GetFileName(file),
						Content = content
					});
				}

				Loader.Load();
			}
			catch (Exception ex)
			{
				Logger.Warn($"Failed processing {Path.GetFileNameWithoutExtension(File)}:\n{ex}");
			}
		}
	}

	public class ZipDevScriptParser : Parser, IBaseProcessor.IParser
	{
		internal const string FOOT = "FindObjectsOfType";

		public override void Process(string file, string input, out string output)
		{
			using (TimeMeasure.New("ScriptParser.Process"))
			{
				try
				{
					if (input.Contains(FOOT))
					{
						Logger.Warn($" Warning! '{Path.GetFileNameWithoutExtension(file)}' uses UnityEngine.GameObject.FindObjectsOfType. That may cause significant performance drops, and/or server stalls. Report to the developer or use at your own discretion!");
					}

					output = input.Replace("PluginTimers", "Timers");
				}
				catch
				{
					output = input;
				}
			}
		}
	}
}
#endif
