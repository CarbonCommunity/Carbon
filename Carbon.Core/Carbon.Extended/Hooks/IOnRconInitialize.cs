using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Facepunch.RCon ), "Initialize" )]
    public class IOnRconInitialize
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "IOnRconInitialize" );
        }
    }
}