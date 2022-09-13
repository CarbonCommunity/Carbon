using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( CodeLock ), "UnlockWithCode" )]
    public class OnCodeEntered
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnCodeEntered" );
        }
    }
}