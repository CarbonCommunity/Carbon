using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseTrapTrigger ), "OnObjectAdded" )]
    public class OnTrapSnapped
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnTrapSnapped" );
        }
    }
}