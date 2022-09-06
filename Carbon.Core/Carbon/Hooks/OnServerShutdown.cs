using Carbon.Core;
using Harmony;

[HarmonyPatch ( typeof ( ServerMgr ), "Shutdown" )]
public class OnServerShutdown
{
    public static void Prefix ()
    {
        HookExecutor.CallStaticHook ( "OnServerShutdown" );
    }
}