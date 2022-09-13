using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ConVar.Chat ), "sayAs" )]
    public class IOnPlayerChat
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "IOnPlayerChat" );
        }
    }
}