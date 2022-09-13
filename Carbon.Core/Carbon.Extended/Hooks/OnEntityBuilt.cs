using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Planner ), "DoBuild" )]
    public class OnEntityBuilt
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnEntityBuilt" );
        }
    }
}