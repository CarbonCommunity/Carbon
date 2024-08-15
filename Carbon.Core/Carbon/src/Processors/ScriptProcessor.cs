using System;
using System.Collections;
using System.IO;
using Carbon.Base;
using Carbon.Components;
using Carbon.Contracts;
using Carbon.Core;

namespace Carbon.Managers;

public class ScriptProcessor : BaseProcessor, IScriptProcessor
{
	public override string Name => "Script Processor";
	public override bool EnableWatcher => !Community.IsConfigReady || Community.Runtime.Config.Watchers.ScriptWatchers;
	public override string Folder => Defines.GetScriptsFolder();
	public override string Extension => ".cs";
	public override float Rate => Community.Runtime.Config.Processors.ScriptProcessingRate;
	public override Type IndexedType => typeof(Script);

	public override void Start()
	{
		BlacklistPattern =
		[
			"backups",
			"debug",
			"cszip_dev"
		];

		base.Start();

		IncludeSubdirectories = Community.Runtime.Config.Watchers.ScriptWatcherOption == SearchOption.AllDirectories;
	}

	public bool AllPendingScriptsComplete()
	{
		foreach (var instance in InstanceBuffer)
		{
			if (instance.Value is Script script)
			{
				if (script.Loader != null && !script.Loader.HasFinished)
				{
					return false;
				}
			}
		}

		return true;
	}
	public bool AllNonRequiresScriptsComplete()
	{
		foreach (var instance in InstanceBuffer)
		{
			if (instance.Value is Script script)
			{
				if (script.Loader != null && !script.Loader.HasRequires && !script.Loader.HasFinished)
				{
					return false;
				}
			}
		}

		return true;
	}
	public bool AllExtensionsComplete()
	{
		foreach (var instance in InstanceBuffer)
		{
			if (instance.Value is Script script)
			{
				if (script.Loader != null && !script.Loader.IsExtension && !script.Loader.HasFinished)
				{
					return false;
				}
			}
		}

		return true;
	}

	void IScriptProcessor.StartCoroutine(IEnumerator coroutine)
	{
		StartCoroutine(coroutine);
	}
	void IScriptProcessor.StopCoroutine(System.Collections.IEnumerator coroutine)
	{
		StopCoroutine(coroutine);
	}

	public class Script : Process, IScriptProcessor.IScript
	{
		public IScriptLoader Loader { get; set; }

		public override IBaseProcessor.IParser Parser => new ScriptParser();

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
				Loader?.Dispose();
			}
			catch (Exception ex)
			{
				Logger.Error($"Error disposing {File}", ex);
			}
		}
		public override void Execute(IBaseProcessor processor)
		{
			try
			{
				ModLoader.GetOrCreateFailedCompilation(File).Clear();

				Loader = new ScriptLoader
				{
					Parser = Parser,
					Mod = Community.Runtime.Plugins,
					Process = this
				};
				Loader.Sources.Add(new BaseSource
				{
					FilePath = File,
					FileName = Path.GetFileName(File),
					ContextFilePath = File,
					ContextFileName = Path.GetFileName(File)
				});
				Loader.Load();
			}
			catch (Exception ex)
			{
				Logger.Warn($"Failed processing {Path.GetFileNameWithoutExtension(File)}:\n{ex}");
			}
		}
	}

	public class ScriptParser : Parser, IBaseProcessor.IParser
	{
		internal const string QuoteReplacer = "[CARBONQUOTE]";
		internal const string Quote = "\\\"";
		internal const string NewLineReplacer = "[CARBONNEWLINE]";
		internal const string NewLine = "\\n";
		internal const string Harmony = "Harmony";
		internal const string FOOT = "FindObjectsOfType";

		public override void Process(string file, string input, out string output)
		{
			using (TimeMeasure.New("ScriptParser.Process"))
			{
				try
				{
					#region Handle Unicode & Quote Escaping

					// if (input.Contains("\\u"))
					// {
					// 	output = Regex.Unescape(input.Replace(Quote, QuoteReplacer).Replace(NewLine, NewLineReplacer)).Replace(QuoteReplacer, Quote).Replace(NewLineReplacer, NewLine);
					// }
					// else output = input;

					#endregion

					if (input.Contains(Harmony))
					{
						Logger.Warn($" Warning! '{Path.GetFileNameWithoutExtension(file)}' uses Harmony. That may cause instability, use at your own discretion!");
					}

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
