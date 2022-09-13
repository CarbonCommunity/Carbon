using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( TerrainGenerator ), "CreateTerrain" )]
    public class OnTerrainCreate
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnTerrainCreate" );
        }
    }
}