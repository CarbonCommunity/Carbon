using Carbon.Core;
using Harmony;

[HarmonyPatch ( typeof ( Bootstrap ), "StartupShared" )]
public class Init
{
    public static void Prefix ()
    {
        CarbonCore.Instance.Init ();
    }
}