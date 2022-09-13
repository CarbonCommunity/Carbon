using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ConnectionAuth ), "OnNewConnection" )]
    public class IOnUserApprove
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "IOnUserApprove" );
        }
    }
}