using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PaintedItemStorageEntity ), "Server_UpdateImage" )]
    public class OnItemPainted
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnItemPainted" );
        }
    }
}