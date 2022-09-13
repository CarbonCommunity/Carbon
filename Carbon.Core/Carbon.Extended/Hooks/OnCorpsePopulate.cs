using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ScarecrowNPC ), "CreateCorpse" )]
    public class OnCorpsePopulate
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnCorpsePopulate" );
        }
    }
}