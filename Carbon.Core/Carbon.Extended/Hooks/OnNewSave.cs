using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( SaveRestore ), "Load" )]
    public class OnNewSave
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnNewSave" );
        }
    }
}