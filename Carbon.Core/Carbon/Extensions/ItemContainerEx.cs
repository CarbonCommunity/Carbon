///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class ItemContainerEx
{
    public static int TakeSkinned ( this ItemContainer container, int itemid, ulong skinId, bool onlyUsableAmounts )
    {
        var num = 0;

        foreach ( var item in container.itemList )
        {
            if ( item.info.itemid == itemid && item.skin == skinId && ( !onlyUsableAmounts || !item.IsBusy () ) )
            {
                num += item.amount;
            }
        }

        return num;
    }
}