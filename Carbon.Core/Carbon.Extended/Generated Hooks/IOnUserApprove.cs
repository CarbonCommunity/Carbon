using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ConnectionAuth ), "OnNewConnection" )]
    public class IOnUserApprove
    {
        public static void Postfix ( Network.Connection connection )
        {
            HookExecutor.CallStaticHook ( "IOnUserApprove" );
        }
    }
}