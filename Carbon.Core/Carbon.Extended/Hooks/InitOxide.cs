using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Bootstrap ), "Init_Tier0" )]
    public class InitOxide
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "InitOxide" );
        }
    }
}