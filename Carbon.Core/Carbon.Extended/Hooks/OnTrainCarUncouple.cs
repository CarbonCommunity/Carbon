using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( TrainCar ), "RPC_WantsUncouple" )]
    public class OnTrainCarUncouple
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnTrainCarUncouple" );
        }
    }
}