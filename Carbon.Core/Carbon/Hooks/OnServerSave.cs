using Carbon.Core;
using Harmony;
using Oxide.Core;
using System.Collections;

[HarmonyPatch ( typeof ( SaveRestore ), "DoAutomatedSave" )]
public class OnServerSave
{
    internal static TimeSince _call;

    public static void Prefix ()
    {
        if ( _call <= 0.5f ) return;

        CarbonCore.Log ( $"Saving Carbon plugins" );
        HookExecutor.CallStaticHook ( "OnServerSave" );
        Interface.Oxide.Permission.SaveData ();
        _call = 0;
    }
}