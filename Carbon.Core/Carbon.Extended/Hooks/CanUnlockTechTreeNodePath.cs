using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( TechTreeData ), "PlayerHasPathForUnlock" )]
    public class CanUnlockTechTreeNodePath
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanUnlockTechTreeNodePath" );
        }
    }
}