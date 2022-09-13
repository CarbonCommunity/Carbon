using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ServerMgr ), "OpenConnection" )]
    public class IOnServerInitialized
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "IOnServerInitialized" );
        }
    }
}