using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( MapEntity ), "ImageUpdate" )]
    public class OnMapImageUpdated
    {
        public static void Postfix ( BaseEntity.RPCMessage msg )
        {
            HookExecutor.CallStaticHook ( "OnMapImageUpdated" );
        }
    }
}