using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( NPCVendingMachine ), "CanPlayerAdmin" )]
    public class CanAdministerVending
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanAdministerVending" );
        }
    }
}