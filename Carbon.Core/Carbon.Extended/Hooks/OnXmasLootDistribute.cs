using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( XMasRefill ), "ServerInit" )]
    public class OnXmasLootDistribute
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnXmasLootDistribute" );
        }
    }
}