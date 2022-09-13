using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PhoneController ), "AnswerPhone" )]
    public class OnPhoneAnswer
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnPhoneAnswer" );
        }
    }
}