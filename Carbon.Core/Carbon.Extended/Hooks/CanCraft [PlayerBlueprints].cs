using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PlayerBlueprints ), "CanCraft" )]
    public class CanCraft [PlayerBlueprints]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanCraft [PlayerBlueprints]" );
        }
    }
}