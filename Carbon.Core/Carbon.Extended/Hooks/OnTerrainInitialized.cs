using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( TerrainMeta ), "PostSetupComponents" )]
    public class OnTerrainInitialized
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnTerrainInitialized" );
        }
    }
}