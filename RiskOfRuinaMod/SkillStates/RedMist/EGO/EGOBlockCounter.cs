using EntityStates;
using EntityStates.Mage;
using RiskOfRuinaMod.Modules;
using RiskOfRuinaMod.Modules.Components;
using RoR2;
using RoR2.Audio;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskOfRuinaMod.SkillStates
{
    class EGOBlockCounter : BaseSkillState
    {
        public float duration = 0.6f;
        public float blinkDuration = 0.5f;
        public float attackStart = 0.25f;
        public bool invul = false;
        public bool fired = false;

        public float damageCounter = 0f;
        public float bonusMult = 1f;

        private Transform modelTransform;
        private CharacterModel characterModel;
        private Animator animator;
        private HurtBoxGroup hurtboxGroup;
        private RedMistStatTracker statTracker;
        private ParticleSystem mistEffect;
        public CameraTargetParams.AimRequest aimRequest;

        public override void OnEnter()
        {
            this.modelTransform = base.GetModelTransform();

            if (this.modelTransform)
            {
                this.animator = this.modelTransform.GetComponent<Animator>();
                this.characterModel = this.modelTransform.GetComponent<CharacterModel>();
                this.hurtboxGroup = this.modelTransform.GetComponent<HurtBoxGroup>();
            }

            statTracker = this.GetComponent<RedMistStatTracker>();

            if (this.characterModel)
            {
                this.characterModel.invisibilityCount++;
            }
            if (this.hurtboxGroup)
            {
                HurtBoxGroup hurtBoxGroup = this.hurtboxGroup;
                int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter + 1;
                hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
            }

            Util.PlaySound("Play_Claw_Ulti_Move", base.gameObject);
            base.PlayAnimation("EGODodge", "EGODodge", "Dodge.playbackRate", this.blinkDuration);

            EffectData effectData = new EffectData();
            effectData.rotation = Quaternion.identity;
            effectData.origin = this.characterBody.corePosition;
            EffectManager.SpawnEffect(statTracker.phaseEffect, effectData, true);

            ChildLocator childLocator = base.GetModelChildLocator();
            if (childLocator)
            {
                this.mistEffect = base.GetComponent<RedMistStatTracker>().mistEffect;
                this.mistEffect.Play();
            }

            invul = true;

            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= this.attackStart && !fired)
            {
                fired = true;
                List<HurtBox> candidates = new List<HurtBox>();
                SphereSearch search = new SphereSearch();
                search.mask = LayerIndex.entityPrecise.mask;
                search.radius = 40f;
                search.ClearCandidates();
                search.origin = this.characterBody.corePosition;
                search.RefreshCandidates();
                search.FilterCandidatesByDistinctHurtBoxEntities();
                TeamMask mask = TeamMask.GetEnemyTeams(base.teamComponent.teamIndex);
                //mask.RemoveTeam(TeamIndex.Neutral);
                search.FilterCandidatesByHurtBoxTeam(mask);
                search.GetHurtBoxes(candidates);

                Util.PlaySound("Play_Kali_Special_Vert_Fin", base.gameObject);

                EffectData effectData = new EffectData();
                effectData.rotation = Quaternion.identity;
                effectData.origin = this.characterBody.footPosition;
                EffectManager.SpawnEffect(statTracker.counterBurst, effectData, true);

                foreach (HurtBox target in candidates)
                {
                    if (target.healthComponent && target.healthComponent.body && target.healthComponent.body != this.characterBody)
                    {
                        DelayedDamage(target);
                    }
                }
            }

            if (base.fixedAge >= this.blinkDuration && invul)
            {
                if (this.characterModel)
                {
                    this.characterModel.invisibilityCount--;
                }
                if (this.hurtboxGroup)
                {
                    HurtBoxGroup hurtBoxGroup = this.hurtboxGroup;
                    int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter - 1;
                    hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
                }
                EffectData effectData = new EffectData();
                effectData.rotation = Quaternion.identity;
                effectData.origin = this.characterBody.corePosition;
                EffectManager.SpawnEffect(statTracker.phaseEffect, effectData, true);
                invul = false;

                this.mistEffect.Stop();
            }

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        private void DelayedDamage(HurtBox target)
        {
            if (NetworkServer.active)
            {
                DamageInfo counterDamage = new DamageInfo()
                {
                    attacker = base.characterBody.gameObject,
                    inflictor = base.characterBody.gameObject,
                    crit = base.RollCrit(),
                    damage = (1.0f + (Config.redMistBuffDamage.Value * (float)this.characterBody.GetBuffCount(Modules.Buffs.RedMistBuff))) * (damageCounter * Modules.StaticValues.blockCounterDamageCoefficient * bonusMult),
                    position = target.transform.position,
                    force = UnityEngine.Vector3.zero,
                    damageType = RoR2.DamageType.Stun1s,
                    damageColorIndex = RoR2.DamageColorIndex.Default,
                    procCoefficient = 1f
                };

                target.healthComponent.TakeDamage(counterDamage);

                GlobalEventManager.instance.OnHitEnemy(counterDamage, target.healthComponent.body.gameObject);
                GlobalEventManager.instance.OnHitAll(counterDamage, target.healthComponent.body.gameObject);
            }

            if (base.isAuthority)
            {
                EffectData effectData = new EffectData();
                effectData.rotation = UnityEngine.Random.rotation;
                effectData.origin = target.healthComponent.body.corePosition;

                EffectManager.SpawnEffect(statTracker.afterimageSlash, effectData, true);
            }
        }

        public override void OnExit()
        {
            this.mistEffect.Stop();

            if (this.characterModel && this.characterModel.invisibilityCount > 0)
            {
                this.characterModel.invisibilityCount--;
            }

            base.cameraTargetParams.cameraParams = Modules.CameraParams.defaultCameraParamsRedMist;
            aimRequest?.Dispose();


            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}
