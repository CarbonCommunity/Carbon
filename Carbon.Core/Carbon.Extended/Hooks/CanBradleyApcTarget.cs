using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BradleyAPC ), "VisibilityTest" )]
    public class CanBradleyApcTarget
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanBradleyApcTarget" );
        }
    }
}