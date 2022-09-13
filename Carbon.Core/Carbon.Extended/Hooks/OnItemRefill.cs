using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ItemModRepair ), "ServerCommand" )]
    public class OnItemRefill
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnItemRefill" );
        }
    }
}