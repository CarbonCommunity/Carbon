using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( RepairBench ), "ChangeSkin" )]
    public class OnItemSkinChange
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnItemSkinChange" );
        }
    }
}