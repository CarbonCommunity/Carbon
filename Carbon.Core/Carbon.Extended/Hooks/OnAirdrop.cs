using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( CargoPlane ), "UpdateDropPosition" )]
    public class OnAirdrop
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnAirdrop" );
        }
    }
}