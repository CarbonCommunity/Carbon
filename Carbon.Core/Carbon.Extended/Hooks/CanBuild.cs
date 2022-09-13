using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Planner ), "DoBuild" )]
    public class CanBuild
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanBuild" );
        }
    }
}