using Oxide.Core;

/*
 *
 * Copyright (c) 2022 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;

public partial class Category_Static
{
	public partial class Static_SaveRestore
	{
		/*
		[Hook.AlwaysPatched]
		[Hook("OnServerSave"), Hook.Category(Hook.Category.Enum.Server)]
		[Hook.Info("Called before the server saves.")]
		[Hook.Patch(typeof(SaveRestore), "DoAutomatedSave")]
		*/

		public class Static_SaveRestore_DoAutomatedSave_eb9f4139698447f594d20fb698c1eb15
		{
			public static Metadata metadata = new Metadata("OnServerSave",
				typeof(SaveRestore), "DoAutomatedSave", new System.Type[] { typeof(bool) });

			static Static_SaveRestore_DoAutomatedSave_eb9f4139698447f594d20fb698c1eb15()
			{
				metadata.SetIdentifier("eb9f4139698447f594d20fb698c1eb15");
				metadata.SetAlwaysPatch(true);
			}

			public static void Prefix(bool AndWait = false)
			{
				Carbon.Logger.Log($"Saving Carbon plugins");
				HookCaller.CallStaticHook("OnServerSave");
				Interface.Oxide.Permission.SaveData();
			}
		}
	}
}