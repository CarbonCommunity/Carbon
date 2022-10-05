///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 


namespace Carbon.Extended
{
	[OxideHook("OnServerSave"), OxideHook.Category(Hook.Category.Enum.Server)]
	[OxideHook.Patch(typeof(SaveRestore), "DoAutomatedSave")]
	public class SaveRestore_DoAutomatedSave
	{
		public static void Prefix(bool AndWait) { }
	}
}
