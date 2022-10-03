///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using System.IO;

namespace Carbon.Core.Processors
{
	public class WebScriptProcessor : BaseProcessor
	{
		public override Type IndexedType => typeof(WebScript);

		public class WebScript : Instance
		{
			internal ScriptLoader _loader;

			public override void Dispose()
			{
				try
				{
					_loader?.Clear();
				}
				catch (Exception ex)
				{
					Logger.Instance.Error($"Error disposing {File}", ex);
				}

				_loader = null;
			}
			public override void Execute()
			{
				try
				{
					_loader = new ScriptLoader();

					CarbonCore.Instance.CorePlugin.webrequest.Enqueue(File, null, (error, result) =>
					{
						Logger.Instance.Log($"Downloaded '{File}': {result.Length}");

						_loader.Sources.Add(result);
						_loader.Load(customFiles: true, customSources: true);
					}, CarbonCore.Instance.CorePlugin);
				}
				catch (Exception ex)
				{
					Logger.Instance.Warn($"Failed processing {Path.GetFileNameWithoutExtension(File)}:\n{ex}");
				}
			}
		}
	}
}
