using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "CreateCorpse" )]
    public class OnPlayerCorpseSpawn
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnPlayerCorpseSpawn" );
        }
    }
}