using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( TechTreeData ), "PlayerCanUnlock" )]
    public class CanUnlockTechTreeNode
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanUnlockTechTreeNode" );
        }
    }
}