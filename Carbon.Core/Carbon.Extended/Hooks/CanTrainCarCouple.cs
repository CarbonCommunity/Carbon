using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( TrainCoupling ), "TryCouple" )]
    public class CanTrainCarCouple
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "CanTrainCarCouple" );
        }
    }
}