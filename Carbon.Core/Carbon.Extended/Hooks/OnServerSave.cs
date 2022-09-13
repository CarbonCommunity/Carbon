using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( SaveRestore ), "DoAutomatedSave" )]
    public class OnServerSave
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnServerSave" );
        }
    }
}