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
    class UdjatMask : RuinaItem
    {
        internal override ConfigEntry<bool> itemEnabled { get; set; }
        internal override string itemName { get; set; } = "RuinaUdjatMask";

        public ItemDef itemDef;

        public float armorIncrease = 5f;
        public float stackIncrease = 5f;

        public override void ItemSetup()
        {
            itemDef = ScriptableObject.CreateInstance<ItemDef>();
            itemDef.name = itemName;
            itemDef.tier = ItemTier.Tier2;
            itemDef.pickupModelPrefab = Assets.udjatMask;
            itemDef.pickupIconSprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texIconPickupRuinaUdjatMask");
            itemDef.nameToken = itemName.ToUpper() + "_NAME";
            itemDef.pickupToken = itemName.ToUpper() + "_PICKUP";
            itemDef.descriptionToken = itemName.ToUpper() + "_DESC";
            itemDef.loreToken = itemName.ToUpper() + "_LORE";
            itemDef.tags = new[]
                {
                    ItemTag.Healing
                };

            var itemDisplayRules = new ItemDisplayRule[0];

            var item = new R2API.CustomItem(itemDef, itemDisplayRules);

            ItemAPI.Add(item);
        }

        public override void HookSetup()
        {
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }

        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            GameObject attacker = damageInfo.attacker;

            if (self && attacker)
            {
                CharacterBody attackerBody = attacker.GetComponent<CharacterBody>();
                CharacterBody victimBody = self.GetComponent<CharacterBody>();
                if (victimBody && attackerBody && attackerBody.teamComponent.teamIndex != victimBody.teamComponent.teamIndex)
                {
                    CharacterMaster master = victimBody.master;
                    if (master)
                    {
                        int count = victimBody.inventory.GetItemCount(ItemCatalog.FindItemIndex(itemName));
                        if (count > 0 && damageInfo.damage > 0)
                        {
                            victimBody.AddTimedBuff(Buffs.udjatBuff, 5f);
                        }
                    }
                }
            }

            orig(self, damageInfo);
        }

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig.Invoke(self);
            int count = base.GetCount(self);
            if (count > 0)
            {
                if (self)
                {
                    if (self.HasBuff(Buffs.udjatBuff))
                    {
                        self.armor += self.GetBuffCount(Buffs.udjatBuff) * (armorIncrease + (stackIncrease * (float)((count) - 1)));
                    }
                }
            }
        }
    }
}