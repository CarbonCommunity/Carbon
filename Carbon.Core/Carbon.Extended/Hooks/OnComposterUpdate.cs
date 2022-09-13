using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Composter ), "UpdateComposting" )]
    public class OnComposterUpdate
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnComposterUpdate" );
        }
    }
}