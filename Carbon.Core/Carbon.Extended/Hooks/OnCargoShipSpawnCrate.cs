using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( CargoShip ), "RespawnLoot" )]
    public class OnCargoShipSpawnCrate
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnCargoShipSpawnCrate" );
        }
    }
}