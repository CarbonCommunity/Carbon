using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseOven ), "FindBurnable" )]
    public class OnFindBurnable
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnFindBurnable" );
        }
    }
}