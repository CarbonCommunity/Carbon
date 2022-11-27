/*
 *
 * Copyright (c) 2022 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;

public partial class Category_Static
{
	public partial class Static_Bootstrap
	{
		/*
		[CarbonHook.AlwaysPatched, CarbonHook.Hidden]
		[CarbonHook("IInit"), CarbonHook.Category(Hook.Category.Enum.Core)]
		[CarbonHook.Patch(typeof(Bootstrap), "StartupShared")]
		*/

		public class Static_Bootstrap_StartupShared_e3acad59160c4885bf724620db14c7e3
		{
			public static Metadata metadata = new Metadata("IInit",
				typeof(Bootstrap), "StartupShared", new System.Type[] { });

			static Static_Bootstrap_StartupShared_e3acad59160c4885bf724620db14c7e3()
			{
				metadata.SetIdentifier("e3acad59160c4885bf724620db14c7e3");
				metadata.SetAlwaysPatch(true);
			}

			public static void Prefix()
			{
				Community.Runtime.Initialize();
			}
		}
	}
}
