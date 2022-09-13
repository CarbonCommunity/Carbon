using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Item ), "LoseCondition" )]
    public class IOnLoseCondition
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "IOnLoseCondition" );
        }
    }
}