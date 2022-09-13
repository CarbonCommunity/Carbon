using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( WaterPurifier ), "ConvertWater" )]
    public class OnWaterPurify
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnWaterPurify" );
        }
    }
}