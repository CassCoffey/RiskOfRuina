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
    class Prescript : RuinaItem
    {
        internal override ConfigEntry<bool> itemEnabled { get; set; }
        internal override string itemName { get; set; } = "RuinaPrescript";

        public ItemDef itemDef;

        public float damageIncrease = 0.01f;

        public override void ItemSetup()
        {
            itemDef = ScriptableObject.CreateInstance<ItemDef>();
            itemDef.name = itemName;
            itemDef.tier = ItemTier.Tier2;
            itemDef.pickupModelPrefab = Assets.prescript;
            itemDef.pickupIconSprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texIconPickupRuinaPrescript");
            itemDef.nameToken = "RUINAPRESCRIPT_NAME";
            itemDef.pickupToken = "RUINAPRESCRIPT_PICKUP";
            itemDef.descriptionToken = "RUINAPRESCRIPT_DESC";
            itemDef.loreToken = "RUINAPRESCRIPT_LORE";
            itemDef.tags = new[]
                {
                    ItemTag.Damage
                };

            var itemDisplayRules = new ItemDisplayRule[0];

            var item = new R2API.CustomItem(itemDef, itemDisplayRules);

            ItemAPI.Add(item);
        }

        public override void HookSetup()
        {
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig.Invoke(self);
            int count = base.GetCount(self);
            if (count > 0)
            {
                List<ItemIndex> items = self.inventory.itemAcquisitionOrder;
                int res = (from x in items
                           select x).Distinct().Count();
                self.damage += (self.baseDamage + self.levelDamage * (self.level - 1f)) * ((float)res * (damageIncrease * (float)count));
            }
        }
    }
}
