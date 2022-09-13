using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( NPCVendingMachine ), "CanPlayerAdmin" )]
    public class CanAdministerVending [NPC]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanAdministerVending [NPC]" );
        }
    }
}