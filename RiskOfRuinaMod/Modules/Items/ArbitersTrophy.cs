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
    class ArbitersTrophy : RuinaItem
    {
        internal override ConfigEntry<bool> itemEnabled { get; set; }
        internal override string itemName { get; set; } = "RuinaArbitersTrophy";

        public ItemDef itemDef;

        public float procChance = 2f;
        public float stackChance = 2f;

        public override void ItemSetup()
        {
            itemDef = ScriptableObject.CreateInstance<ItemDef>();
            itemDef.name = itemName;
            itemDef.tier = ItemTier.Tier3;
            itemDef.pickupModelPrefab = Assets.arbiterTrophy;
            itemDef.pickupIconSprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texIconPickupRuinaArbitersTrophy");
            itemDef.nameToken = "ARBITERTROPHY_NAME";
            itemDef.pickupToken = "ARBITERTROPHY_PICKUP";
            itemDef.descriptionToken = "ARBITERTROPHY_DESC";
            itemDef.loreToken = "ARBITERTROPHY_LORE";
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
                                int stacks = victimBody.GetBuffCount(Modules.Buffs.lockResistBuff);

                                if (stacks <= 4 && victimBody.GetBuffCount(Modules.Buffs.lockDebuff) == 0)
                                {
                                    if (NetworkServer.active)
                                    {
                                        victimBody.AddTimedBuff(Modules.Buffs.lockDebuff, 5f - (float)stacks, 1);
                                        victimBody.AddBuff(Modules.Buffs.lockResistBuff);
                                    }

                                    Transform modelTransform = victimBody.modelLocator.modelTransform;

                                    if (victimBody && modelTransform)
                                    {
                                        TemporaryOverlay overlay = victimBody.gameObject.AddComponent<TemporaryOverlay>();
                                        overlay.duration = 5f - (float)stacks;
                                        overlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                                        overlay.animateShaderAlpha = true;
                                        overlay.destroyComponentOnEnd = true;
                                        overlay.originalMaterial = Assets.mainAssetBundle.LoadAsset<Material>("matChains");
                                        overlay.AddToCharacerModel(modelTransform.GetComponent<CharacterModel>());
                                    }

                                    EntityStateMachine component = victimBody.GetComponent<EntityStateMachine>();

                                    if (component != null)
                                    {
                                        LockState state = new LockState
                                        {
                                            duration = 5f - (float)stacks
                                        };
                                        component.SetState(state);
                                    }
                                }

                                int duration = 5 - stacks;

                                GameObject effect = null;

                                switch (duration)
                                {
                                    case 5:
                                        effect = Assets.lockEffect5s;
                                        break;
                                    case 4:
                                        effect = Assets.lockEffect4s;
                                        break;
                                    case 3:
                                        effect = Assets.lockEffect3s;
                                        break;
                                    case 2:
                                        effect = Assets.lockEffect2s;
                                        break;
                                    case 1:
                                        effect = Assets.lockEffect1s;
                                        break;
                                    default:
                                        effect = Assets.lockEffectBreak;
                                        break;
                                }

                                if (victimBody.healthComponent && victimBody.healthComponent.combinedHealthFraction <= 0)
                                {
                                    // killed them, don't linger
                                    effect = Assets.lockEffectBreak;
                                }

                                EffectData effectData = new EffectData();
                                effectData.rotation = Util.QuaternionSafeLookRotation(Vector3.zero);
                                effectData.origin = victimBody.corePosition;

                                EffectManager.SpawnEffect(effect, effectData, true);
                            }
                        }
                    }
                }
            }

            orig(self, damageInfo, victim);
        }
    }
}
