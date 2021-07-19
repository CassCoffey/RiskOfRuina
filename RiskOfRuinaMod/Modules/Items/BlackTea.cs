using BepInEx.Configuration;
using R2API;
using RiskOfRuinaMod.Modules.Misc;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskOfRuinaMod.Modules.Items
{
    class BlackTea : RuinaItem
    {
        internal override ConfigEntry<bool> itemEnabled { get; set; }
        internal override string itemName { get; set; } = "RuinaBlackTea";

        public ItemDef itemDef;

        public float procChance = 10f;
        public float stackChance = 5f;

        public override void ItemSetup()
        {
            itemDef = ScriptableObject.CreateInstance<ItemDef>();
            itemDef.name = itemName;
            itemDef.tier = ItemTier.Tier1;
            itemDef.pickupModelPrefab = Assets.arbiterTrophy;
            itemDef.pickupIconSprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texRedMistUtilityIcon");
            itemDef.nameToken = "RUINABLACKTEA_NAME";
            itemDef.pickupToken = "RUINABLACKTEA_PICKUP";
            itemDef.descriptionToken = "RUINABLACKTEA_DESC";
            itemDef.loreToken = "RUINABLACKTEA_LORE";
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
                            float chance = procChance + (stackChance * (float)(count - 1));
                            if (Util.CheckRoll(chance * damageInfo.procCoefficient, master))
                            {
                                if (damageInfo.dotIndex != Modules.DoTCore.FairyIndex)
                                {
                                    InflictDotInfo inflictDotInfo = default(InflictDotInfo);
                                    inflictDotInfo.attackerObject = damageInfo.attacker;
                                    inflictDotInfo.victimObject = victim;
                                    inflictDotInfo.dotIndex = Modules.DoTCore.FairyIndex;
                                    inflictDotInfo.duration = 20f;
                                    inflictDotInfo.damageMultiplier = 0;
                                    InflictDotInfo inflictDotInfo2 = inflictDotInfo;
                                    DotController.InflictDot(ref inflictDotInfo2);
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
