using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( IOEntity.IORef ), "Clear" )]
    public class OnIORefCleared
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnIORefCleared" );
        }
    }
}