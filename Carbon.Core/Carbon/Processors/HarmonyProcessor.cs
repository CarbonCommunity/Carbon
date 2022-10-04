///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using System.IO;

namespace Carbon.Core.Processors
{
	public class HarmonyProcessor : BaseProcessor
	{
		public override bool EnableWatcher => CarbonCore.IsConfigReady ? CarbonCore.Instance.Config.HarmonyWatchers : true;
		public override string Folder => CarbonCore.GetHarmonyFolder();
		public override string Extension => ".dll";
		public override Type IndexedType => typeof(Harmony);

		public class Harmony : Instance
		{
			public override void Dispose()
			{
				CarbonLoader.UnloadCarbonMod(Path.GetFileNameWithoutExtension(File));
			}
			public override void Execute()
			{
				try
				{
					CarbonLoader.LoadCarbonMod(File, true);
				}
				catch (Exception ex)
				{
					Logger.Warn($"Failed processing {Path.GetFileNameWithoutExtension(File)}:\n{ex}");
				}
			}
		}
	}
}
