using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( RepairBench ), "ChangeSkin" )]
    public class OnItemSkinChange
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnItemSkinChange" );
        }
    }
}