using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseMountable ), "DismountPlayer" )]
    public class OnEntityDismounted [lite]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnEntityDismounted [lite]" );
        }
    }
}