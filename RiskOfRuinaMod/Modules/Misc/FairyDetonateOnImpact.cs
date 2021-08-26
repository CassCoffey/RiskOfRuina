using UnityEngine;
using RoR2;
using RoR2.Projectile;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace RiskOfRuinaMod.Modules.Misc
{
    [RequireComponent(typeof(ProjectileController))]
    public class DetonateFairyOnImpact : NetworkBehaviour, IProjectileImpactBehavior
    {
        [SyncVar]
        public float radius;

        private ProjectileController controller;

        private void Awake()
        {
            this.controller = base.GetComponent<ProjectileController>();
        }

        public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
        {
            if (NetworkServer.active)
            {
                float radiusSqr = this.radius * this.radius;

                Vector3 position = base.transform.position;
                for (TeamIndex teamIndex = TeamIndex.Neutral; teamIndex < TeamIndex.Count; teamIndex += 1)
                {
                    if (teamIndex != controller.teamFilter.teamIndex)
                    {
                        this.FairyBurst(TeamComponent.GetTeamMembers(teamIndex), radiusSqr, position);
                    }
                }
            }
        }

        private void FairyBurst(IEnumerable<TeamComponent> recipients, float radiusSqr, Vector3 currentPosition)
        {
            if (!NetworkServer.active)
            {
                return;
            }

            foreach (TeamComponent teamComponent in recipients)
            {
                if ((teamComponent.transform.position - currentPosition).sqrMagnitude <= radiusSqr)
                {
                    CharacterBody charBody = teamComponent.body;
                    if (charBody)
                    {
                        DotController selfDotController = DotController.FindDotController(charBody.gameObject);

                        if (selfDotController && selfDotController.HasDotActive(Modules.DoTCore.FairyIndex))
                        {
                            for (int i = 0; i < selfDotController.dotStackList.Count; i++)
                            {
                                DotController.DotStack stack = selfDotController.dotStackList[i];
                                if (stack.dotIndex == Modules.DoTCore.FairyIndex)
                                {
                                    DamageInfo fairyDamage = new DamageInfo()
                                    {
                                        attacker = stack.attackerObject,
                                        inflictor = stack.attackerObject,
                                        crit = stack.attackerObject.GetComponent<CharacterBody>().RollCrit(),
                                        damage = stack.attackerObject.GetComponent<CharacterBody>().damage * Modules.StaticValues.fairyDebuffCoefficient,
                                        position = charBody.corePosition,
                                        force = UnityEngine.Vector3.zero,
                                        damageType = RoR2.DamageType.Generic,
                                        damageColorIndex = RoR2.DamageColorIndex.Bleed,
                                        dotIndex = Modules.DoTCore.FairyIndex,
                                        procCoefficient = 0.75f
                                    };

                                    charBody.healthComponent.TakeDamage(fairyDamage);

                                    GlobalEventManager.instance.OnHitEnemy(fairyDamage, charBody.gameObject);
                                    GlobalEventManager.instance.OnHitAll(fairyDamage, charBody.gameObject);
                                }
                            }

                            // Remove all stacks
                            //for (int i = selfDotController.dotStackList.Count - 1; i >= 0; i--)
                            //{
                            //    DotController.DotStack stack = selfDotController.dotStackList[i];
                            //    if (stack.dotIndex == Modules.DoTCore.FairyIndex)
                            //    {
                            //        selfDotController.RemoveDotStackAtServer(i);
                            //    }
                            //}

                            EffectData effectData = new EffectData();
                            effectData.rotation = Util.QuaternionSafeLookRotation(UnityEngine.Vector3.zero);
                            effectData.origin = charBody.corePosition;

                            EffectManager.SpawnEffect(Modules.Assets.fairyProcEffect, effectData, false);
                        }
                    }
                }
            }
        }
    }
}
