using Carbon.Core;
using Harmony;

[HarmonyPatch ( typeof ( Bootstrap ), "StartupShared" )]
public class Bootstrap_StartupShared
{
    public static void Prefix ()
    {
        CarbonCore.Instance.Init ();
    }
}