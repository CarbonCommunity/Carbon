using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ScientistNPC ), "EquipWeapon" )]
    public class OnNpcEquipWeapon
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnNpcEquipWeapon" );
        }
    }
}