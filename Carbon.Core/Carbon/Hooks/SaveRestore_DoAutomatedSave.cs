using Carbon.Core;
using Harmony;

[HarmonyPatch ( typeof ( SaveRestore ), "DoAutomatedSave" )]
public class SaveRestore_DoAutomatedSave
{
    public static void Prefix ()
    {
        HookExecutor.CallStaticHook ( "OnServerSave" );
    }
}