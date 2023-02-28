using System;
using System.IO;
using Carbon.Base;
using Carbon.Contracts;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Processors;

public class WebScriptProcessor : BaseProcessor, IWebScriptProcessor
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
				Logger.Error($"Error disposing {File}", ex);
			}

			_loader = null;
		}
		public override void Execute()
		{
			try
			{
				_loader = new ScriptLoader();

				CommunityCommon.CommonRuntime.CorePlugin.webrequest.Enqueue(File, null, (error, result) =>
				{
					Logger.Log($"Downloaded '{File}': {result.Length}");

					_loader.Source = result;
					_loader.Load();
				}, CommunityCommon.CommonRuntime.CorePlugin);
			}
			catch (Exception ex)
			{
				Logger.Warn($"Failed processing {Path.GetFileNameWithoutExtension(File)}:\n{ex}");
			}
		}
	}
}
