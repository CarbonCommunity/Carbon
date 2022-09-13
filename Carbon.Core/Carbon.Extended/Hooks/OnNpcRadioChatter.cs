using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ScientistNPC ), "PlayRadioChatter" )]
    public class OnNpcRadioChatter
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnNpcRadioChatter" );
        }
    }
}