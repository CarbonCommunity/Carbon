using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( SprayCan ), "CreateSpray" )]
    public class OnSprayCreate
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnSprayCreate" );
        }
    }
}