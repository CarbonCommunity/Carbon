///
/// Copyright (c) 2022 kasvoton
/// All rights reserved
/// 

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Carbon.Core.Modules
{
    public class StackManagerModule : BaseModule<StackManagerConfig>
    {
        public override string Name => "StackManager";

        public override void OnEnabledServerInit ()
        {
            base.OnEnabledServerInit ();

            if ( ItemManager.itemList == null ) return;

            foreach ( var category in Config.Categories )
            {
                foreach ( var item in ItemManager.itemList )
                {
                    if ( item.category != category.Key || Config.Blacklist.Contains ( item.shortname ) || Config.Items.ContainsKey ( item.shortname ) ) continue;

                    item.stackable = Mathf.CeilToInt ( item.stackable * category.Value );
                }
            }

            foreach ( var item in ItemManager.itemList )
            {
                if ( !Config.Items.ContainsKey ( item.shortname ) ) continue;

                item.stackable = Mathf.CeilToInt ( item.stackable * Config.Items [ item.shortname ] );
            }

            Puts ( "Item stacks patched" );
        }
        public override void OnDisabledServerInit ()
        {
            base.OnDisabledServerInit ();

            Puts ( "Rolling back item manager" );

            foreach ( var category in Config.Categories )
            {
                foreach ( var item in ItemManager.itemList )
                {
                    if ( item.category != category.Key || Config.Blacklist.Contains ( item.shortname ) || Config.Items.ContainsKey ( item.shortname ) ) continue;

                    item.stackable = Mathf.CeilToInt ( item.stackable / category.Value );
                }
            }

            foreach ( var item in ItemManager.itemList )
            {
                if ( !Config.Items.TryGetValue ( item.shortname, out var multiply ) ) continue;

                item.stackable = Mathf.CeilToInt ( item.stackable / multiply );
            }
        }
    }

    public class StackManagerConfig
    {
        public HashSet<string> Blacklist = new HashSet<string>
        {
            "water",
            "water.salt"
        };

        public Dictionary<ItemCategory, float> Categories = new Dictionary<ItemCategory, float>
        {
            { ItemCategory.Ammunition, 1f },
            { ItemCategory.Attire, 1f },
            { ItemCategory.Component, 1f },
            { ItemCategory.Construction, 1f },
            { ItemCategory.Electrical, 1f },
            { ItemCategory.Food, 1f },
            { ItemCategory.Fun, 1f },
            { ItemCategory.Items, 1f },
            { ItemCategory.Medical, 1f },
            { ItemCategory.Misc, 1f },
            { ItemCategory.Resources, 1f },
            { ItemCategory.Tool, 1f },
            { ItemCategory.Traps, 1f },
            { ItemCategory.Weapon, 1f }
        };

        public Dictionary<string, float> Items = new Dictionary<string, float>
        {
            { "explosive.timed", 1f }
        };
    }
}