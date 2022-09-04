using Harmony;
using Carbon.Core;

[HarmonyPatch ( typeof ( ServerMgr ), "OpenConnection" )]
public class ServerMgr_OpenConnection
{
    public static void Postfix ()
    {
        CarbonCore.Instance._installProcessors ();
        CarbonCore.Instance.RefreshConsoleInfo ();

        HookExecutor.CallStaticHook ( "OnServerInitialized" );
    }
}