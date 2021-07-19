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
            itemDef.pickupModelPrefab = Assets.arbiterTrophy;
            itemDef.pickupIconSprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texRedMistUtilityIcon");
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
            On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEvent_OnHitEnemy;
        }

        private void GlobalEvent_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, UnityEngine.GameObject victim)
        {
            GameObject attacker = damageInfo.attacker;

            if (self && attacker)
            {
                CharacterBody attackerBody = attacker.GetComponent<CharacterBody>();
                CharacterBody victimBody = victim.GetComponent<CharacterBody>();
                if (attackerBody.teamComponent.teamIndex != victimBody.teamComponent.teamIndex)
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

                                    DamageInfo bonusDamage = new DamageInfo()
                                    {
                                        attacker = damageInfo.attacker,
                                        inflictor = damageInfo.inflictor,
                                        crit = damageInfo.crit,
                                        damage = damageInfo.damage * bonus,
                                        position = damageInfo.position,
                                        force = UnityEngine.Vector3.zero,
                                        damageType = DamageType.Generic,
                                        damageColorIndex = RoR2.DamageColorIndex.Default,
                                        procCoefficient = 0f,
                                    };

                                    victimBody.healthComponent.TakeDamage(bonusDamage);
                                }
                            }
                        }
                    }
                }
            }

            orig(self, damageInfo, victim);
        }
    }
}