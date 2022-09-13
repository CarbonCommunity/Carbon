using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseNetworkable ), "Spawn" )]
    public class OnEntitySpawned
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnEntitySpawned" );
        }
    }
}