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

        public override void OnEnabled ( bool initialized )
        {
            base.OnEnabled ( initialized );

            if ( !initialized || ItemManager.itemList == null ) return;

            foreach ( var category in Config.Categories )
            {
                if ( StackManagerConfig.IsValueInvalid ( category.Value ) ) continue;

                foreach ( var item in ItemManager.itemList )
                {
                    if ( item.category != category.Key || Config.Blacklist.Contains ( item.shortname ) || Config.Items.ContainsKey ( item.shortname ) ) continue;

                    item.stackable = Mathf.CeilToInt ( item.stackable * category.Value );
                }
            }

            foreach ( var item in ItemManager.itemList )
            {
                if ( !Config.Items.ContainsKey ( item.shortname ) ) continue;

                var multiplier = Config.Items [ item.shortname ];

                if ( StackManagerConfig.IsValueInvalid ( multiplier ) ) continue;

                item.stackable = Mathf.CeilToInt ( item.stackable * multiplier );
            }

            Puts ( "Item stacks patched" );
        }
        public override void OnDisabled ( bool initialized )
        {
            base.OnDisabled ( initialized );

            if ( !initialized ) return;

            Puts ( "Rolling back item manager" );

            foreach ( var category in Config.Categories )
            {
                if ( StackManagerConfig.IsValueInvalid ( category.Value ) ) continue;

                foreach ( var item in ItemManager.itemList )
                {
                    if ( item.category != category.Key || Config.Blacklist.Contains ( item.shortname ) || Config.Items.ContainsKey ( item.shortname ) ) continue;

                    item.stackable = Mathf.CeilToInt ( item.stackable / category.Value );
                }
            }

            foreach ( var item in ItemManager.itemList )
            {
                if ( !Config.Items.TryGetValue ( item.shortname, out var multiplier ) ) continue;

                if ( StackManagerConfig.IsValueInvalid ( multiplier ) ) continue;

                item.stackable = Mathf.CeilToInt ( item.stackable / multiplier );
            }
        }
    }

    public class StackManagerConfig
    {
        public static bool IsValueInvalid ( float value )
        {
            return value < 0.1f;
        }

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