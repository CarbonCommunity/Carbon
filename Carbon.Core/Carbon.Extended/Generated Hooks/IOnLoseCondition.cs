using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Item ), "LoseCondition" )]
    public class IOnLoseCondition
    {
        public static void Postfix ( System.Single amount )
        {
            HookExecutor.CallStaticHook ( "IOnLoseCondition" );
        }
    }
}