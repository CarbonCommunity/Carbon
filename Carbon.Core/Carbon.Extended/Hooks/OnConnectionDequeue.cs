using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ConnectionQueue ), "RemoveConnection" )]
    public class OnConnectionDequeue
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnConnectionDequeue" );
        }
    }
}