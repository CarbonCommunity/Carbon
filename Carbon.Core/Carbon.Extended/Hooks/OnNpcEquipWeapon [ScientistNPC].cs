using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ScientistNPC ), "EquipWeapon" )]
    public class OnNpcEquipWeapon [ScientistNPC]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnNpcEquipWeapon [ScientistNPC]" );
        }
    }
}