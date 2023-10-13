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

namespace Carbon.Managers;

public class WebScriptProcessor : BaseProcessor, IWebScriptProcessor
{
	public override string Name => "WebScript Processor";
	public override Type IndexedType => typeof(WebScript);

	public class WebScript : Process
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
		public override void Execute(IBaseProcessor processor)
		{
			try
			{
				_loader = new ScriptLoader();

				Community.Runtime.CorePlugin.webrequest.Enqueue(File, null, (error, result) =>
				{
					Logger.Log($"Downloaded '{File}': {result.Length}");

					_loader.Sources.Add(new BaseSource
					{
						ContextFilePath = File,
						ContextFileName = Path.GetFileName(File),
						FilePath = File,
						FileName = Path.GetFileName(File),
						Content = result
					});
					_loader.Load();
				}, Community.Runtime.CorePlugin);
			}
			catch (Exception ex)
			{
				Logger.Warn($"Failed processing {Path.GetFileNameWithoutExtension(File)}:\n{ex}");
			}
		}
	}
}
