using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PhoneController ), "CallPhone" )]
    public class OnPhoneDial
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnPhoneDial" );
        }
    }
}