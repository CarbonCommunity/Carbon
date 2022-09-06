using Carbon.Core;
using Harmony;

[HarmonyPatch ( typeof ( ConVar.Global ), "quit" )]
public class Quit
{
    public static void Prefix ( ConsoleSystem.Arg args )
    {
        CarbonCore.Instance.HarmonyProcessor.Clear ();
        CarbonCore.Instance.PluginProcessor.Clear ();
    }
}