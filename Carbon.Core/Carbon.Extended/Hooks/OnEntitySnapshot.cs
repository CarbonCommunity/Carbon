using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "SendEntitySnapshot" )]
    public class OnEntitySnapshot
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnEntitySnapshot" );
        }
    }
}