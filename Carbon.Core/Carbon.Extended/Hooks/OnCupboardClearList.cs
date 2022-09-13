using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BuildingPrivlidge ), "ClearList" )]
    public class OnCupboardClearList
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnCupboardClearList" );
        }
    }
}