using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ScientistNPC ), "PlayRadioChatter" )]
    public class OnNpcRadioChatter [ScientistNPC]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnNpcRadioChatter [ScientistNPC]" );
        }
    }
}