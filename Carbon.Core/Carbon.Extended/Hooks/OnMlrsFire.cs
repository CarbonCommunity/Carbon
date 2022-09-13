using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( MLRS ), "Fire" )]
    public class OnMlrsFire
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnMlrsFire" );
        }
    }
}