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
    class WorkshopAmmo : RuinaItem
    {
        internal override ConfigEntry<bool> itemEnabled { get; set; }
        internal override string itemName { get; set; } = "RuinaWorkshopAmmo";

        public ItemDef itemDef;

        public float damageIncrease = 0.25f;
        public float stackIncrease = 0.10f;

        public override void ItemSetup()
        {
            itemDef = ScriptableObject.CreateInstance<ItemDef>();
            itemDef.name = itemName;
            itemDef.tier = ItemTier.Tier2;
            itemDef.pickupModelPrefab = Assets.workshopAmmo;
            itemDef.pickupIconSprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texIconPickupRuinaWorkshopAmmo");
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
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
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
                    CharacterMaster master = attackerBody.master;
                    if (master)
                    {
                        int count = attackerBody.inventory.GetItemCount(ItemCatalog.FindItemIndex(itemName));
                        if (count > 0)
                        {
                            float distance = Vector3.Distance(attackerBody.corePosition, victimBody.corePosition);

                            if (distance >= 10f)
                            {
                                if (NetworkServer.active)
                                {
                                    float stackCalc = (damageIncrease + (stackIncrease * (float)(count - 1)));
                                    float bonus = Mathf.Clamp(Mathf.Lerp(0f, stackCalc, (distance - 10f) / 100f), 0f, stackCalc);

                                    damageInfo.damage += damageInfo.damage * bonus;
                                    damageInfo.damageColorIndex = DamageColorIndex.Nearby;
                                }
                            }
                        }
                    }
                }
            }

            orig(self, damageInfo);
        }
    }
}