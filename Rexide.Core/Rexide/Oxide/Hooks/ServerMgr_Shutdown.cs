using Harmony;
using Rexide.Core.Harmony;

[HarmonyPatch ( typeof ( ServerMgr ), "Shutdown" )]
public class ServerMgr_Shutdown
{
    public static void Prefix ()
    {
        HookExecutor.CallStaticHook("OnServerShutdown");
    }
}