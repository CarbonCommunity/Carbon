using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseTrapTrigger ), "OnObjectAdded" )]
    public class OnTrapSnapped
    {
        public static bool Prefix ( UnityEngine.GameObject obj, UnityEngine.Collider col )
        {
            return HookExecutor.CallStaticHook ( "OnTrapSnapped" ) == null;
        }
    }
}