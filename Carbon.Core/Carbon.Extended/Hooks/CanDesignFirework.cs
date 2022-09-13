using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PatternFirework ), "PlayerCanModify" )]
    public class CanDesignFirework
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "CanDesignFirework" );
        }
    }
}