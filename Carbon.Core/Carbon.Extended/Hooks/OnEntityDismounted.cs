using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseMountable ), "DismountPlayer" )]
    public class OnEntityDismounted
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnEntityDismounted" );
        }
    }
}