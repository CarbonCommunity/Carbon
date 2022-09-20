using Facepunch;
using Harmony;
using System.Collections.Generic;

[HarmonyPatch ( typeof ( ItemContainer ), "Take" )]
public class ItemContainer_Take
{
    public static bool Prefix ( List<Item> collect, int itemid, int iAmount, out int __result, ref ItemContainer __instance )
    {
        var num = 0;
        if ( iAmount == 0 )
        {
            __result = num;
        }

        var list = Pool.GetList<Item> ();
        foreach ( var item in __instance.itemList )
        {
            if ( item.info.itemid != itemid || item.skin != 0 ) continue;

            int num2 = iAmount - num;
            if ( num2 > 0 )
            {
                if ( item.amount > num2 )
                {
                    item.MarkDirty ();
                    item.amount -= num2;
                    num += num2;

                    var item2 = ItemManager.CreateByItemID ( itemid, 1, 0UL );
                    item2.amount = num2;
                    item2.CollectedForCrafting ( __instance.playerOwner );

                    if ( collect != null )
                    {
                        collect.Add ( item2 );
                        break;
                    }
                    break;
                }
                else
                {
                    if ( item.amount <= num2 )
                    {
                        num += item.amount;
                        list.Add ( item );
                        collect?.Add ( item );
                    }

                    if ( num == iAmount )
                    {
                        break;
                    }
                }
            }
        }

        foreach ( var item3 in list )
        {
            item3.RemoveFromContainer ();
        }

        Pool.FreeList ( ref list );
        __result = num;
        return false;
    }
}