using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PlayerBelt ), "DropActive" )]
    public class OnPlayerDropActiveItem
    {
        public static void Postfix ( UnityEngine.Vector3 position, UnityEngine.Vector3 velocity , ref PlayerBelt __instance )
        {
            HookExecutor.CallStaticHook ( "OnPlayerDropActiveItem" );
        }
    }
}