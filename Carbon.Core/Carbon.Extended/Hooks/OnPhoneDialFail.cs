using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PhoneController ), "OnDialFailed" )]
    public class OnPhoneDialFail
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnPhoneDialFail" );
        }
    }
}