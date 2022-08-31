using Harmony;
using Carbon.Core.Harmony;

[HarmonyPatch ( typeof ( ServerMgr ), "OpenConnection" )]
public class ServerMgr_OpenConnection
{
    public static void Postfix ()
    {
        // PlayerPrefs.SetString ( Harmony_Load.CARBON_LOADED, string.Empty );
        HookExecutor.CallStaticHook ( "OnServerInitialized" );
    }
}