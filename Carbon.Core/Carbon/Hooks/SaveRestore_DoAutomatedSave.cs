using Harmony;
using Carbon.Core.Harmony;

[HarmonyPatch ( typeof ( SaveRestore ), "DoAutomatedSave" )]
public class SaveRestore_DoAutomatedSave
{
    public static void Postfix ()
    {
        HookExecutor.CallStaticHook ( "OnServerSave" );
    }
}