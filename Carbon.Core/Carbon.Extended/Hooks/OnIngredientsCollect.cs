using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ItemCrafter ), "CollectIngredients" )]
    public class OnIngredientsCollect
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnIngredientsCollect" );
        }
    }
}