using BepInEx.Configuration;
using R2API;
using RiskOfRuinaMod.Modules.Misc;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskOfRuinaMod.Modules.Items
{
    class Reverberation : RuinaItem
    {
        internal override ConfigEntry<bool> itemEnabled { get; set; }
        internal override string itemName { get; set; } = "RuinaReverberation";

        public ItemDef itemDef;

        public override void ItemSetup()
        {
            itemDef = ScriptableObject.CreateInstance<ItemDef>();
            itemDef.name = itemName;
            itemDef.tier = ItemTier.Tier3;
            itemDef.pickupModelPrefab = Assets.reverberation;
            itemDef.pickupIconSprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texIconPickupRuinaReverberation");
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
            On.RoR2.OverlapAttack.Fire += OverlapAttack_Fire;
            On.RoR2.BulletAttack.Fire += BulletAttack_Fire;
            On.RoR2.BlastAttack.Fire += BlastAttack_Fire;
        }

        private bool OverlapAttack_Fire(On.RoR2.OverlapAttack.orig_Fire orig, OverlapAttack self, List<HurtBox> hitResults)
        {
            GameObject attacker = self.attacker;

            if (attacker)
            {
                CharacterBody attackerBody = attacker.GetComponent<CharacterBody>();
                if (attackerBody)
                {
                    CharacterMaster master = attackerBody.master;
                    if (master && attackerBody.inventory)
                    {
                        int count = attackerBody.inventory.GetItemCount(ItemCatalog.FindItemIndex(itemName));

                        if (count > 0)
                        {
                            Vector3 pos = self.hitBoxGroup.hitBoxes[0].transform.position;
                            Vector3 half = (self.hitBoxGroup.hitBoxes[0].transform.localScale / 2f) * (1f + (0.25f * ((float)count - 1f)));
                            Quaternion rotate = self.hitBoxGroup.hitBoxes[0].transform.rotation;

                            Collider[] projectiles = Physics.OverlapBox(pos, half, rotate, LayerIndex.projectile.mask);

                            for (int i = 0; i < projectiles.Length; i++)
                            {
                                ProjectileController projectile = projectiles[i].GetComponent<ProjectileController>();
                                ProjectileDamage projDamage = projectiles[i].GetComponent<ProjectileDamage>();
                                if (projectile && projDamage)
                                {
                                    TeamComponent projectileTeam = projectile.owner.GetComponent<TeamComponent>();
                                    if (projectileTeam)
                                    {
                                        if (projectileTeam.teamIndex != attackerBody.teamComponent.teamIndex)
                                        {
                                            EffectData effectData = new EffectData();
                                            effectData.origin = projectile.transform.position;

                                            EffectManager.SpawnEffect(Modules.Assets.blockEffect, effectData, false);

                                            FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
                                            {
                                                projectilePrefab = projectile.gameObject,
                                                position = projectile.transform.position,
                                                rotation = Util.QuaternionSafeLookRotation(attackerBody.inputBank.GetAimRay().direction),
                                                owner = attacker,
                                                damage = projDamage.damage + self.damage,
                                                force = projDamage.force,
                                                crit = projDamage.crit,
                                                target = projectile.owner
                                            };

                                            ProjectileManager.instance.FireProjectile(fireProjectileInfo);

                                            Util.PlaySound("Play_Defense_Guard", attacker);

                                            CharacterBody.Destroy(projectile.gameObject);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return orig(self, hitResults);
        }

        private void BulletAttack_Fire(On.RoR2.BulletAttack.orig_Fire orig, BulletAttack self)
        {
            GameObject attacker = self.owner;

            if (attacker)
            {
                CharacterBody attackerBody = attacker.GetComponent<CharacterBody>();
                if (attackerBody)
                {
                    CharacterMaster master = attackerBody.master;
                    if (master && attackerBody.inventory)
                    {
                        int count = attackerBody.inventory.GetItemCount(ItemCatalog.FindItemIndex(itemName));

                        if (count > 0)
                        {
                            RaycastHit info;

                            if (Physics.SphereCast(new Ray(self.origin, self.aimVector), 0.75f * (1f + (0.25f * ((float)count - 1f))), out info, self.maxDistance, LayerIndex.projectile.mask))
                            {
                                ProjectileController projectile = info.collider.GetComponent<ProjectileController>();
                                ProjectileDamage projDamage = info.collider.GetComponent<ProjectileDamage>();
                                if (projectile && projDamage)
                                {
                                    TeamComponent projectileTeam = projectile.owner.GetComponent<TeamComponent>();
                                    if (projectileTeam)
                                    {
                                        if (projectileTeam.teamIndex != attackerBody.teamComponent.teamIndex)
                                        {
                                            EffectData effectData = new EffectData();
                                            effectData.origin = projectile.transform.position;

                                            EffectManager.SpawnEffect(Modules.Assets.blockEffect, effectData, false);

                                            FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
                                            {
                                                projectilePrefab = projectile.gameObject,
                                                position = projectile.transform.position,
                                                rotation = Util.QuaternionSafeLookRotation(self.aimVector),
                                                owner = attacker,
                                                damage = projDamage.damage + self.damage,
                                                force = projDamage.force,
                                                crit = projDamage.crit
                                            };

                                            ProjectileManager.instance.FireProjectile(fireProjectileInfo);

                                            Util.PlaySound("Play_Defense_Guard", attacker);

                                            CharacterBody.Destroy(projectile.gameObject);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            orig(self);
        }

        private RoR2.BlastAttack.Result BlastAttack_Fire(On.RoR2.BlastAttack.orig_Fire orig, BlastAttack self)
        {
            GameObject attacker = self.attacker;

            if (attacker)
            {
                CharacterBody attackerBody = attacker.GetComponent<CharacterBody>();
                if (attackerBody)
                {
                    CharacterMaster master = attackerBody.master;
                    if (master && attackerBody.inventory)
                    {
                        int count = attackerBody.inventory.GetItemCount(ItemCatalog.FindItemIndex(itemName));

                        if (count > 0)
                        {
                            Collider[] projectiles = Physics.OverlapSphere(self.position, self.radius * (1f + (0.25f * ((float)count - 1f))), LayerIndex.projectile.mask);

                            for (int i = 0; i < projectiles.Length; i++)
                            {
                                ProjectileController projectile = projectiles[i].GetComponent<ProjectileController>();
                                ProjectileDamage projDamage = projectiles[i].GetComponent<ProjectileDamage>();
                                if (projectile && projDamage)
                                {
                                    TeamComponent projectileTeam = projectile.owner.GetComponent<TeamComponent>();
                                    if (projectileTeam)
                                    {
                                        if (projectileTeam.teamIndex != attackerBody.teamComponent.teamIndex)
                                        {
                                            EffectData effectData = new EffectData();
                                            effectData.origin = projectile.transform.position;

                                            EffectManager.SpawnEffect(Modules.Assets.blockEffect, effectData, false);

                                            FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
                                            {
                                                projectilePrefab = projectile.gameObject,
                                                position = projectile.transform.position,
                                                rotation = Util.QuaternionSafeLookRotation((projectile.transform.position - self.position).normalized),
                                                owner = attacker,
                                                damage = projDamage.damage + self.baseDamage,
                                                force = projDamage.force,
                                                crit = projDamage.crit
                                            };

                                            ProjectileManager.instance.FireProjectile(fireProjectileInfo);

                                            Util.PlaySound("Play_Defense_Guard", attacker);

                                            CharacterBody.Destroy(projectile.gameObject);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return orig(self);
        }
    }
}
