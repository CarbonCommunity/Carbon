///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;

namespace Carbon.Extended
{
    [OxideHook ( "OnServerSave" ), OxideHook.Category ( Hook.Category.Enum.Player )]
    [OxideHook.Patch ( typeof ( SaveRestore ), "DoAutomatedSave" )]
    public class SaveRestore_DoAutomatedSave
    {
        public static void Prefix () { }
    }
}