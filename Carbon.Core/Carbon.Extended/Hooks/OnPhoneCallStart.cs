using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PhoneController ), "BeginCall" )]
    public class OnPhoneCallStart
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnPhoneCallStart" );
        }
    }
}