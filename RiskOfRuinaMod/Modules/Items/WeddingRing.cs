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
    class WeddingRing : RuinaItem
    {
        internal override ConfigEntry<bool> itemEnabled { get; set; }
        internal override string itemName { get; set; } = "RuinaWeddingRing";

        public ItemDef itemDef;

        public float damageIncrease = 0.1f;
        public float stackIncrease = 0.05f;
        public float range = 25f;

        public override void ItemSetup()
        {
            itemDef = ScriptableObject.CreateInstance<ItemDef>();
            itemDef.name = itemName;
            itemDef.tier = ItemTier.Tier2;
            itemDef.pickupModelPrefab = Assets.weddingRing;
            itemDef.pickupIconSprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texIconPickupRuinaWeddingRing");
            itemDef.nameToken = itemName.ToUpper() + "_NAME";
            itemDef.pickupToken = itemName.ToUpper() + "_PICKUP";
            itemDef.descriptionToken = itemName.ToUpper() + "_DESC";
            itemDef.loreToken = itemName.ToUpper() + "_LORE";
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
                int allyCount = 0;
                foreach (TeamComponent teamComponent in TeamComponent.GetTeamMembers(self.teamComponent.teamIndex))
                {
                    if ((teamComponent.transform.position - self.corePosition).sqrMagnitude <= (range * range))
                    {
                        CharacterBody charBody = teamComponent.body;
                        if (charBody && charBody != self && charBody.inventory)
                        {
                            allyCount += charBody.inventory.GetItemCount(ItemCatalog.FindItemIndex(itemName));
                        }
                    }
                }
                if (allyCount > 0)
                {
                    self.damage += (self.baseDamage + self.levelDamage * (self.level - 1f)) * (damageIncrease + (stackIncrease * (float)((count + allyCount) - 1)));
                }
            }
        }
    }
}