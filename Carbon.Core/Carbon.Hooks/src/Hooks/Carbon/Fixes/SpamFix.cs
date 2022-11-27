using UnityEngine;

/*
 *
 * Copyright (c) 2022 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;

public partial class Category_Fixes
{
	public partial class Fixes_Debug
	{
		/*
		[CarbonHook.AlwaysPatched, CarbonHook.Hidden]
		[CarbonHook("IDebugLogWarning"), CarbonHook.Category(Hook.Category.Enum.Core)]
		[CarbonHook.Patch(typeof(Debug), "LogWarning", true, typeof(object), typeof(Object))]
		*/

		public class Fixes_Debug_LogWarning_e75bcb98e64645bd96afc60742d1171b
		{
			public static Metadata metadata = new Metadata("IDebugLogWarning",
				typeof(Debug), "LogWarning", new System.Type[] { typeof(object), typeof(UnityEngine.Object) });

			static Fixes_Debug_LogWarning_e75bcb98e64645bd96afc60742d1171b()
			{
				metadata.SetIdentifier("e75bcb98e64645bd96afc60742d1171b");
				metadata.SetAlwaysPatch(true);
				metadata.SetHidden(true);
			}

			public static bool Prefix(object message, UnityEngine.Object context)
			{
				try
				{
					var log = message.ToString();

					if (
						log.Contains("failed to sample navmesh at position") ||
						log.Contains("not close enough to the NavMesh") ||
						log.Contains("performing complete refresh, please wait")
					) return false;
				}
				catch { }

				return true;
			}
		}
	}
}