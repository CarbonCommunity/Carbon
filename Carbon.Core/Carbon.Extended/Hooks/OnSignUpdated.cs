using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( CarvablePumpkin ), "UpdateSign" )]
    public class OnSignUpdated
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnSignUpdated" );
        }
    }
}