using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( HumanNPC ), "CreateCorpse" )]
    public class OnCorpsePopulate [HumanNPC]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnCorpsePopulate [HumanNPC]" );
        }
    }
}