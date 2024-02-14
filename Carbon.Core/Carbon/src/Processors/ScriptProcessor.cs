using System;
using System.Collections;
using System.IO;
using Carbon.Base;
using Carbon.Components;
using Carbon.Contracts;
using Carbon.Core;

/*
 *
 * Copyright (c) 2022-2024 Carbon Community
 * All rights reserved.
 *
 */

namespace Carbon.Managers;

public class ScriptProcessor : BaseProcessor, IScriptProcessor
{
	public override string Name => "Script Processor";
	public override bool EnableWatcher => !Community.IsConfigReady || Community.Runtime.Config.ScriptWatchers;
	public override string Folder => Defines.GetScriptFolder();
	public override string Extension => ".cs";
	public override Type IndexedType => typeof(Script);

	public override void Start()
	{
		BlacklistPattern = new[]
		{
			"backups",
			"debug",
			"cszip_dev"
		};

		base.Start();

		IncludeSubdirectories = Community.Runtime.Config.ScriptWatcherOption == SearchOption.AllDirectories;
	}

	public bool AllPendingScriptsComplete()
	{
		foreach (var instance in InstanceBuffer)
		{
			if (instance.Value is Script script)
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
			if (instance.Value is Script script)
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
			if (instance.Value is Script script)
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
	void IScriptProcessor.StopCoroutine(System.Collections.IEnumerator coroutine)
	{
		StopCoroutine(coroutine);
	}

	public class Script : Process, IScriptProcessor.IScript
	{
		public IScriptLoader Loader { get; set; }

		public override IBaseProcessor.IParser Parser => new ScriptParser();

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

			Loader = null;
		}
		public override void Execute(IBaseProcessor processor)
		{
			try
			{
				ModLoader.FailedMods.RemoveAll(x => x.File == File);

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

					output = input.Replace("PluginTimers", "Timers")
						.Replace("using Harmony;", "using HarmonyLib;")
						.Replace("HarmonyInstance.Create", "new Harmony")
						.Replace("HarmonyInstance", "Harmony");
				}
				catch
				{
					output = input;
				}
			}
		}
	}
}
