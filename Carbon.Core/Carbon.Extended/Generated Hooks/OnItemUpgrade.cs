using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ItemModUpgrade ), "ServerCommand" )]
    public class OnItemUpgrade
    {
        public static void Postfix ( Item item, System.String command, BasePlayer player )
        {
            HookExecutor.CallStaticHook ( "OnItemUpgrade" );
        }
    }
}