using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( NPCPlayer ), "EquipWeapon" )]
    public class OnNpcEquipWeapon [NPCPlayer]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnNpcEquipWeapon [NPCPlayer]" );
        }
    }
}