using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ItemCrafter ), "CraftItem" )]
    public class FixItemKeyId [patch]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "FixItemKeyId [patch]" );
        }
    }
}