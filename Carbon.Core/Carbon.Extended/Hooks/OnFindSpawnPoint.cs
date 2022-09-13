using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ServerMgr ), "FindSpawnPoint" )]
    public class OnFindSpawnPoint
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnFindSpawnPoint" );
        }
    }
}