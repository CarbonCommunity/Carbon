using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ResourceContainer ), "StartLootingContainer" )]
    public class CanLootEntity [ResourceContainer]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanLootEntity [ResourceContainer]" );
        }
    }
}