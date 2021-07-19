using EntityStates;
using EntityStates.Mage;
using RiskOfRuinaMod.Modules;
using RiskOfRuinaMod.Modules.Components;
using RoR2;
using RoR2.Audio;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskOfRuinaMod.SkillStates
{
    class EGOBlock : BaseSkillState
    {
        public float duration = 1f;
        public float invulStart = 0f;
        public float invulEnd = 0.35f;
        public float hitBonus = 0.6f;
        public bool invul = false;
        public bool blockOut = false;

        public float damageCounter = 0f;

        protected RedMistEmotionComponent emotionComponent;
        protected RedMistStatTracker statTracker;
        private Transform modelTransform;
        private HurtBoxGroup hurtboxGroup;
        private CharacterModel characterModel;
        private ParticleSystem mistEffect;

        private float originalHeight;
        private float originalRadius;


        public override void OnEnter()
        {
            this.emotionComponent = base.gameObject.GetComponent<RedMistEmotionComponent>();
            this.statTracker = base.gameObject.GetComponent<RedMistStatTracker>();
            this.modelTransform = base.GetModelTransform();

            if (this.modelTransform)
            {
                this.characterModel = this.modelTransform.GetComponent<CharacterModel>();
                this.hurtboxGroup = this.modelTransform.GetComponent<HurtBoxGroup>();
            }

            if (this.characterModel)
            {
                this.characterModel.invisibilityCount++;
            }

            Util.PlaySound("Play_DaeChi", base.gameObject);
            base.PlayAnimation("EGODodge", "EGODodge", "Dodge.playbackRate", this.invulEnd);

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

            if (NetworkServer.active)
            {
                base.characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
                invul = true;
            }

            base.OnEnter();

            On.RoR2.GlobalEventManager.OnHitEnemy += OnHit;

            base.cameraTargetParams.cameraParams = Modules.CameraParams.HorizontalSlashCameraParamsRedMist;
            base.cameraTargetParams.aimMode = CameraTargetParams.AimType.Aura;

            CapsuleCollider col = (CapsuleCollider)base.characterBody.mainHurtBox.collider;
            originalHeight = col.height;
            originalRadius = col.radius;

            col.height = originalHeight * 2f;
            col.radius = originalRadius * 25f;
        }

        public void OnHit(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, UnityEngine.GameObject victim)
        {
            if (victim == this.gameObject && invul)
            {
                base.PlayAnimation("EGODodge", "EGODodge", "Dodge.playbackRate", hitBonus);
                Util.PlaySound("Play_Defense_Guard", base.gameObject);

                invulEnd = this.fixedAge + hitBonus;
                duration = invulEnd + hitBonus;

                GameObject attacker = damageInfo.attacker;
                if (attacker && attacker.GetComponent<CharacterBody>())
                {
                    damageCounter += damageInfo.damage;

                    CharacterBody component = attacker.GetComponent<CharacterBody>();

                    if (NetworkServer.active)
                    {
                        DamageInfo counterDamage = new DamageInfo()
                        {
                            attacker = base.characterBody.gameObject,
                            inflictor = base.characterBody.gameObject,
                            crit = base.RollCrit(),
                            damage = (1.0f + (Config.redMistBuffDamage.Value * (float)this.characterBody.GetBuffCount(Modules.Buffs.RedMistBuff))) * (damageInfo.damage * Modules.StaticValues.blockCounterDamageCoefficient),
                            position = attacker.transform.position,
                            force = UnityEngine.Vector3.zero,
                            damageType = RoR2.DamageType.Generic,
                            damageColorIndex = RoR2.DamageColorIndex.Default,
                            procCoefficient = 1f
                        };

                        component.healthComponent.TakeDamage(counterDamage);

                        GlobalEventManager.instance.OnHitEnemy(counterDamage, attacker);
                        GlobalEventManager.instance.OnHitAll(counterDamage, attacker);
                    }

                    if (base.isAuthority)
                    {
                        EffectData effectData = new EffectData();
                        effectData.rotation = UnityEngine.Random.rotation;
                        effectData.origin = component.healthComponent.body.corePosition;

                        EffectManager.SpawnEffect(statTracker.afterimageSlash, effectData, true);

                        Vector3 dirToTarget = (component.footPosition - this.characterBody.footPosition);
                        dirToTarget.y = 0f;

                        effectData = new EffectData();
                        effectData.rotation = Quaternion.LookRotation(dirToTarget.normalized, Vector3.up);
                        effectData.origin = this.characterBody.footPosition + (UnityEngine.Random.Range(0f, 2.5f) * dirToTarget.normalized);
                        
                        EffectManager.SpawnEffect(statTracker.afterimageBlock, effectData, true);
                    }
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (NetworkServer.active && base.fixedAge < this.invulEnd)
            {
                if (!base.characterBody.HasBuff(RoR2Content.Buffs.HiddenInvincibility))
                {
                    base.characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
                }
            }

            if (NetworkServer.active && base.fixedAge >= this.invulEnd && invul)
            {
                if (base.characterBody.HasBuff(RoR2Content.Buffs.HiddenInvincibility))
                {
                    base.characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
                }

                if (this.characterModel)
                {
                    this.characterModel.invisibilityCount--;
                }
                EffectData effectData = new EffectData();
                effectData.rotation = Quaternion.identity;
                effectData.origin = this.characterBody.corePosition;
                EffectManager.SpawnEffect(statTracker.phaseEffect, effectData, true);

                this.mistEffect.Stop();

                invul = false;

                // Counter Time
                if (damageCounter > 0f)
                {
                    if (base.isAuthority)
                    {
                        this.outer.SetNextState(new EGOBlockCounter
                        {
                            damageCounter = this.damageCounter
                        });
                    }
                } else
                {
                    base.cameraTargetParams.cameraParams = Modules.CameraParams.defaultCameraParamsRedMist;
                    base.cameraTargetParams.aimMode = CameraTargetParams.AimType.Standard;
                }
            }

            if (this.damageCounter > 0f && !base.inputBank.skill3.down && base.isAuthority)
            {
                this.outer.SetNextState(new EGOBlockCounter
                {
                    damageCounter = this.damageCounter
                });
            }

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            if (NetworkServer.active && base.characterBody.HasBuff(RoR2Content.Buffs.HiddenInvincibility))
            {
                base.characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
            }

            CapsuleCollider col = (CapsuleCollider)base.characterBody.mainHurtBox.collider;
            col.height = originalHeight;
            col.radius = originalRadius;

            On.RoR2.GlobalEventManager.OnHitEnemy -= OnHit;

            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}
