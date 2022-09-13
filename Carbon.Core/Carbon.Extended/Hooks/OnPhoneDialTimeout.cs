using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PhoneController ), "TimeOutDialing" )]
    public class OnPhoneDialTimeout
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnPhoneDialTimeout" );
        }
    }
}