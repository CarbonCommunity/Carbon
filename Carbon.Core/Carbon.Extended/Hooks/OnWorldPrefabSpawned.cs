using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( World ), "Spawn" )]
    public class OnWorldPrefabSpawned
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnWorldPrefabSpawned" );
        }
    }
}