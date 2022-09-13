using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ConVar.Chat ), "sayAs" )]
    public class IOnPlayerCommand
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "IOnPlayerCommand" );
        }
    }
}