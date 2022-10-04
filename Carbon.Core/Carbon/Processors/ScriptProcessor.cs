///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using System.IO;

namespace Carbon.Core.Processors
{
	public class ScriptProcessor : BaseProcessor
	{
		public override bool EnableWatcher => CarbonCore.IsConfigReady ? CarbonCore.Instance.Config.ScriptWatchers : true;
		public override string Folder => CarbonCore.GetPluginsFolder();
		public override string Extension => ".cs";
		public override Type IndexedType => typeof(Script);

		public bool AllPendingScriptsComplete()
		{
			foreach (var instance in InstanceBuffer)
			{
				if (instance.Value is Script script)
				{
					if (script._loader != null && !script._loader.HasFinished) return false;
				}
			}

			return true;
		}

		public class Script : Instance
		{
			internal ScriptLoader _loader;

			public override Parser Parser => new ScriptParser();

			public override void Dispose()
			{
				try
				{
					_loader?.Clear();
				}
				catch (Exception ex)
				{
					Logger.Error($"Error disposing {File}", ex);
				}

				_loader = null;
			}
			public override void Execute()
			{
				try
				{
					_loader = new ScriptLoader();
					_loader.Parser = Parser;
					_loader.Files.Add(File);
					_loader.Load(true);
				}
				catch (Exception ex)
				{
					Logger.Warn($"Failed processing {Path.GetFileNameWithoutExtension(File)}:\n{ex}");
				}
			}
		}

		public class ScriptParser : Parser
		{
			public override void Process(string input, out string output)
			{
				output = input.Replace(".IPlayer", ".AsIPlayer()");
			}
		}
	}
}
