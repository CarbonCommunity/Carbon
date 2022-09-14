using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "RespawnAt" )]
    public class OnPlayerRespawned
    {
        public static void Postfix ( UnityEngine.Vector3 position, UnityEngine.Quaternion rotation )
        {
            HookExecutor.CallStaticHook ( "OnPlayerRespawned" );
        }
    }
}