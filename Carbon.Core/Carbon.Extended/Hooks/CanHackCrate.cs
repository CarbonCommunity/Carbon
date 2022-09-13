using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( HackableLockedCrate ), "RPC_Hack" )]
    public class CanHackCrate
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "CanHackCrate" );
        }
    }
}