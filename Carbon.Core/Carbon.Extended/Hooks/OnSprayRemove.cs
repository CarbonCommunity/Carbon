using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( SprayCanSpray ), "Server_RequestWaterClear" )]
    public class OnSprayRemove
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnSprayRemove" );
        }
    }
}