using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( CodeLock ), "TryUnlock" )]
    public class CanUnlock [CodeLock]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanUnlock [CodeLock]" );
        }
    }
}