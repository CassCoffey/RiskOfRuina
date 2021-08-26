using BepInEx.Configuration;
using R2API;
using R2API.Utils;
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
    class BackwardsClock : RuinaEquipment
    {
        internal override ConfigEntry<bool> equipEnabled { get; set; }
        internal override string equipName { get; set; } = "RuinaBackwardsClock";

        public EquipmentDef equipDef;

        public override void EquipSetup()
        {
            equipDef = ScriptableObject.CreateInstance<EquipmentDef>();
            equipDef.name = equipName;
            equipDef.appearsInMultiPlayer = true;
            equipDef.appearsInSinglePlayer = false;
            equipDef.isLunar = true;
            equipDef.pickupModelPrefab = Assets.backwardsClock;
            equipDef.pickupIconSprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texIconPickupRuinaBackwardsClock");
            equipDef.nameToken = equipName.ToUpper() + "_NAME";
            equipDef.pickupToken = equipName.ToUpper() + "_PICKUP";
            equipDef.descriptionToken = equipName.ToUpper() + "_DESC";
            equipDef.loreToken = equipName.ToUpper() + "_LORE";
            equipDef.enigmaCompatible = false;
            equipDef.canDrop = true;
            equipDef.cooldown = 0f;

            var equipDisplayRules = new ItemDisplayRule[0];

            var equipment = new R2API.CustomEquipment(equipDef, equipDisplayRules);

            ItemAPI.Add(equipment);
        }

        public override void HookSetup()
        {
            On.RoR2.EquipmentSlot.PerformEquipmentAction += EquipmentSlot_PerformEquipmentAction;
        }

        private bool EquipmentSlot_PerformEquipmentAction(On.RoR2.EquipmentSlot.orig_PerformEquipmentAction orig, EquipmentSlot self, EquipmentDef equipmentIndex)
        {
            if (equipmentIndex == equipDef)
            {
                if (NetworkServer.active)
                {
                    foreach (PlayerCharacterMasterController deadCharacter in GetAllDeadCharacters())
                    {
                        GameObject bodyPrefab = BodyCatalog.GetBodyPrefab(deadCharacter.networkUser.NetworkbodyIndexPreference);
                        if (bodyPrefab != null)
                        {
                            deadCharacter.master.bodyPrefab = bodyPrefab;
                        }
                        deadCharacter.master.Respawn(Reflection.GetFieldValue<Vector3>(deadCharacter.master, "deathFootPosition"), deadCharacter.master.transform.rotation);
                        EffectManager.SpawnEffect(Resources.Load<GameObject>("Prefabs/Effects/HippoRezEffect"), new EffectData
                        {
                            origin = deadCharacter.master.GetBody().footPosition,
                            rotation = deadCharacter.master.gameObject.transform.rotation
                        }, true);
                    }

                    // Kill user
                    DamageInfo clockDamage = new DamageInfo()
                    {
                        attacker = null,
                        inflictor = null,
                        crit = true,
                        damage = self.characterBody.healthComponent.combinedHealth + self.characterBody.healthComponent.fullBarrier,
                        position = self.transform.position,
                        force = UnityEngine.Vector3.zero,
                        damageType = RoR2.DamageType.BypassOneShotProtection,
                        damageColorIndex = RoR2.DamageColorIndex.Default,
                        procCoefficient = 0f
                    };

                    self.characterBody.inventory.SetEquipmentIndex(EquipmentIndex.None);

                    // No countering death
                    if (self.characterBody.HasBuff(RoR2Content.Buffs.HiddenInvincibility))
                    {
                        self.characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
                    }

                    self.characterBody.healthComponent.TakeDamage(clockDamage);
                    GlobalEventManager.instance.OnHitAll(clockDamage, self.gameObject);

                    // Stun Enemies
                    for (TeamIndex teamIndex = TeamIndex.Neutral; teamIndex < TeamIndex.Count; teamIndex += 1)
                    {
                        if (teamIndex != self.characterBody.teamComponent.teamIndex)
                        {
                            foreach (TeamComponent teamComponent in TeamComponent.GetTeamMembers(teamIndex))
                            {
                                CharacterBody charBody = teamComponent.body;
                                if (charBody)
                                {
                                    EntityStateMachine component = charBody.GetComponent<EntityStateMachine>();

                                    if (component != null)
                                    {
                                        EntityStates.StunState state = new EntityStates.StunState
                                        {
                                            duration = 5f,
                                            stunDuration = 5f
                                        };
                                        component.SetState(state);
                                    }
                                }
                            }
                        }
                    }
                }
                return true;
            }

            return orig(self, equipmentIndex); // dont forget this !
        }

        private List<PlayerCharacterMasterController> GetAllDeadCharacters()
        {
            List<PlayerCharacterMasterController> list = new List<PlayerCharacterMasterController>();
            foreach (PlayerCharacterMasterController playerCharacterMasterController in PlayerCharacterMasterController.instances)
            {
                NetworkUser networkUser = playerCharacterMasterController.networkUser;
                if (playerCharacterMasterController.isConnected && (networkUser.master.IsDeadAndOutOfLivesServer() || networkUser.master.bodyPrefab != BodyCatalog.GetBodyPrefab(networkUser.NetworkbodyIndexPreference)))
                {
                    list.Add(networkUser.master.playerCharacterMasterController);
                }
            }

            return list;
        }
    }
}