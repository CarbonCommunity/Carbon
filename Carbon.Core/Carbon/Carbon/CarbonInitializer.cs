using System.Reflection;
using UnityEngine;

namespace Carbon.Core
{
	public class CarbonInitializer : IHarmonyModHooks
	{
		public void OnLoaded(OnHarmonyModLoadedArgs args)
		{
			var oldMod = PlayerPrefs.GetString(Harmony_Load.CARBON_LOADED);

			if (!Assembly.GetExecutingAssembly().FullName.StartsWith(oldMod))
			{
				CarbonCore.Instance?.UnInit();
				HarmonyLoader.TryUnloadMod(oldMod);
				Logger.WarnFormat($"Unloaded previous: {oldMod}");
				CarbonCore.Instance = null;
			}

			Logger.Format("Initializing...");

			if (CarbonCore.Instance == null) CarbonCore.Instance = new CarbonCore();
			else CarbonCore.Instance?.UnInit();

			CarbonCore.Instance.Init();
		}

		public void OnUnloaded(OnHarmonyModUnloadedArgs args) { }
	}
}
