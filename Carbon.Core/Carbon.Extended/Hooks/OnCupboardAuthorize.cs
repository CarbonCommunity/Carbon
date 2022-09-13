using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ItemModDeployable ), "OnDeployed" )]
    public class OnCupboardAuthorize
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnCupboardAuthorize" );
        }
    }
}