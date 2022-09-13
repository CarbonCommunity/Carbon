using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PhoneController ), "UpdatePhoneName" )]
    public class OnPhoneNameUpdate
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnPhoneNameUpdate" );
        }
    }
}