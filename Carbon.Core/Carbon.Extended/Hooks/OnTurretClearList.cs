using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( AutoTurret ), "ClearList" )]
    public class OnTurretClearList
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnTurretClearList" );
        }
    }
}