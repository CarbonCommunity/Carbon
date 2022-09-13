using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PhoneController ), "CanReceiveCall" )]
    public class CanReceiveCall
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanReceiveCall" );
        }
    }
}