using Carbon.Core;
using Harmony;

[HarmonyPatch ( typeof ( ServerMgr ), "Shutdown" )]
public class OnServerShutdown
{
    internal static TimeSince _call;

    public static void Prefix ()
    {
        if ( _call <= 0.5f ) return;

        CarbonCore.Log ( $"Saving Carbon plugins & shutting down" );
        HookExecutor.CallStaticHook ( "OnServerSave" );
        HookExecutor.CallStaticHook ( "OnServerShutdown" );

        CarbonCore.Instance.HarmonyProcessor.Clear ();
        CarbonCore.Instance.ScriptProcessor.Clear ();
        _call = 0;
    }
}