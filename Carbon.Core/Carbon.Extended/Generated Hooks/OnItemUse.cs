using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Item ), "UseItem" )]
    public class OnItemUse
    {
        public static void Postfix ( System.Int32 amountToConsume , ref Item __instance )
        {
            HookExecutor.CallStaticHook ( "OnItemUse" );
        }
    }
}