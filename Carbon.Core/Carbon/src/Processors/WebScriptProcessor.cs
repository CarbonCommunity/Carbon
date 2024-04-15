using System;
using System.IO;
using Carbon.Base;
using Carbon.Contracts;

/*
 *
 * Copyright (c) 2022-2024 Carbon Community
 * All rights reserved.
 *
 */

namespace Carbon.Managers;

public class WebScriptProcessor : BaseProcessor, IWebScriptProcessor
{
	public override string Name => "WebScript Processor";
	public override Type IndexedType => typeof(WebScript);

	public class WebScript : Process
	{
		public IScriptLoader Loader { get; set; }

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
				Loader = new ScriptLoader();

				Community.Runtime.CorePlugin.webrequest.Enqueue(File, null, (error, result) =>
				{
					Logger.Log($"Downloaded '{File}': {result.Length}");

					Loader.Sources.Add(new BaseSource
					{
						ContextFilePath = File,
						ContextFileName = Path.GetFileName(File),
						FilePath = File,
						FileName = Path.GetFileName(File),
						Content = result
					});
					Loader.Load();
				}, Community.Runtime.CorePlugin);
			}
			catch (Exception ex)
			{
				Logger.Warn($"Failed processing {Path.GetFileNameWithoutExtension(File)}:\n{ex}");
			}
		}
	}
}
