using Harmony;
using Carbon.Core;

[HarmonyPatch ( typeof ( ServerMgr ), "OpenConnection" )]
public class OnServerInitialized
{
    internal static TimeSince _call;

    public static void Postfix ()
    {
        if ( _call <= 0.5f ) return;

        HookExecutor.CallStaticHook ( "OnServerInitialized" );
        _call = 0;
    }
}