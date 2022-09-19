using Carbon.Core;
using Harmony;
using Oxide.Core;

namespace Carbon.Extended
{
    [Hook ( "OnItemRemove", typeof ( object ) ), Hook.Category ( Hook.Category.Enum.Item )]
    [Hook.Parameter ( "this", typeof ( Item ) )]
    [Hook.Info ( "Called before an item is destroyed." )]
    [Hook.Info ( "Return a non-null value stop item from being destroyed." )]
    [Hook.Patch ( typeof ( Item ), "Remove" )]
    public class Item_Remove
    {
        public static bool Prefix ( float fTime, ref Item __instance )
        {
            if ( __instance.removeTime > 0f )
            {
                return true;
            }

            return Interface.CallHook ( "OnItemRemove", __instance ) == null;
        }
    }
}