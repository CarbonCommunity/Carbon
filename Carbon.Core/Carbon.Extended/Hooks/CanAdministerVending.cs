using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( VendingMachine ), "CanPlayerAdmin" )]
    public class CanAdministerVending
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanAdministerVending" );
        }
    }
}