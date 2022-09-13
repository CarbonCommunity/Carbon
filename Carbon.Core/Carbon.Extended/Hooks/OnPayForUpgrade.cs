using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BuildingBlock ), "PayForUpgrade" )]
    public class OnPayForUpgrade
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnPayForUpgrade" );
        }
    }
}