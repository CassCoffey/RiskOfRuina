using BepInEx.Configuration;
using R2API;
using RiskOfRuinaMod.Modules.Misc;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskOfRuinaMod.Modules.Items
{
    class MoonlightStone : RuinaItem
    {
        internal override ConfigEntry<bool> itemEnabled { get; set; }
        internal override string itemName { get; set; } = "RuinaMoonlightStone";

        public ItemDef itemDef;

        public override void ItemSetup()
        {
            itemDef = ScriptableObject.CreateInstance<ItemDef>();
            itemDef.name = itemName;
            itemDef.tier = ItemTier.Tier1;
            itemDef.pickupModelPrefab = Assets.moonlightStone;
            itemDef.pickupIconSprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texIconPickupRuinaMoonlightStone");
            itemDef.nameToken = itemName.ToUpper() + "_NAME";
            itemDef.pickupToken = itemName.ToUpper() + "_PICKUP";
            itemDef.descriptionToken = itemName.ToUpper() + "_DESC";
            itemDef.loreToken = itemName.ToUpper() + "_LORE";
            itemDef.tags = new[]
                {
                    ItemTag.Utility
                };

            var itemDisplayRules = new ItemDisplayRule[0];

            var item = new R2API.CustomItem(itemDef, itemDisplayRules);

            ItemAPI.Add(item);
        }

        public override void HookSetup()
        {
            On.RoR2.CharacterBody.OnInventoryChanged += new On.RoR2.CharacterBody.hook_OnInventoryChanged(this.CharacterBody_OnInventoryChanged);
        }

        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            if (self && self.inventory)
            {
                self.AddItemBehavior<MoonlightStoneBehavior>(base.GetCount(self));
            }
            orig.Invoke(self);
        }
    }
}