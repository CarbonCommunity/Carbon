using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ContainerIOEntity ), "PlayerOpenLoot" )]
    public class CanLootEntity [ContainerIOEntity]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanLootEntity [ContainerIOEntity]" );
        }
    }
}