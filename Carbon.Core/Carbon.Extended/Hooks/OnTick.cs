using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ServerMgr ), "DoTick" )]
    public class OnTick
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnTick" );
        }
    }
}