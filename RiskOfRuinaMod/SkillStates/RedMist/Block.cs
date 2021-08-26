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
    class Block : BaseSkillState
    {
        public float duration = 0.7f;
        public float invulEnd = 0.35f;
        public float hitBonus = 0.6f;
        public bool invul = false;
        public bool blockOut = false;

        public float damageCounter = 0f;
        public float bonusMult = 1f;
        public float stockBonus = 0.4f;
        public int hits = 0;

        protected TemporaryOverlay iframeOverlay;

        protected RedMistEmotionComponent emotionComponent;
        protected RedMistStatTracker statTracker;

        private float originalHeight;
        private float originalRadius;


        public override void OnEnter()
        {
            this.emotionComponent = base.gameObject.GetComponent<RedMistEmotionComponent>();
            this.statTracker = base.gameObject.GetComponent<RedMistStatTracker>();

            if (base.skillLocator.utility.stock > 1)
            {
                this.bonusMult += (base.skillLocator.utility.stock - 1) * this.stockBonus;
            }

            AddOverlay(invulEnd);

            if (NetworkServer.active)
            {
                base.characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
            }

            invul = true;

            base.OnEnter();

            Util.PlaySound("Ruina_Swipe", base.gameObject);
            base.PlayAnimation("FullBody, Override", "BlockIn");

            RiskOfRuinaNetworkManager.ServerOnHit += OnHit;

            CapsuleCollider col = (CapsuleCollider)base.characterBody.mainHurtBox.collider;
            originalHeight = col.height;
            originalRadius = col.radius;

            col.height = originalHeight * 1.5f;
            col.radius = originalRadius * 10f;
        }

        public void OnHit(float damage, UnityEngine.GameObject attacker, UnityEngine.GameObject victim)
        {
            if (victim == this.gameObject && invul)
            {
                Util.PlaySound("Play_Defense_Guard", base.gameObject);

                if (attacker && attacker.GetComponent<CharacterBody>() && attacker.GetComponent<CharacterBody>() != this.characterBody)
                {
                    CharacterBody component = attacker.GetComponent<CharacterBody>();
                    Vector3 dirToTarget = (component.footPosition - this.characterBody.footPosition);
                    dirToTarget.y = 0f;
                    dirToTarget.Normalize();
                    Vector3 forward = characterDirection.forward;
                    Vector3 up = base.transform.up;
                    Vector3 right = Vector3.Cross(up, forward).normalized;
                    Vector2 directionVector = new Vector2(Vector3.Dot(dirToTarget, forward), Vector3.Dot(dirToTarget, right)).normalized;

                    if (directionVector.x >= 0.5f)
                    {
                        base.PlayAnimation("FullBody, Override", "BlockHit");
                    }
                    else if (directionVector.x <= -0.5f)
                    {
                        base.PlayAnimation("FullBody, Override", "BlockHitBack");
                    }
                    else if (directionVector.y >= 0.5f)
                    {
                        base.PlayAnimation("FullBody, Override", "BlockHitRight");
                    }
                    else if (directionVector.y <= -0.5f)
                    {
                        base.PlayAnimation("FullBody, Override", "BlockHitLeft");
                    }
                    else
                    {
                        base.PlayAnimation("FullBody, Override", "BlockHit");
                    }

                    invulEnd = this.fixedAge + hitBonus;
                    duration = invulEnd + hitBonus;

                    float modDamage = damage;

                    if (attacker && attacker.GetComponent<CharacterBody>())
                    {
                        modDamage = damage;
                        if (RiskOfRuinaPlugin.kombatArenaInstalled)
                        {
                            if (RiskOfRuinaPlugin.KombatGamemodeActive() && this.characterBody.master && RiskOfRuinaPlugin.KombatIsDueling(this.characterBody.master))
                            {
                                modDamage = damage * StaticValues.counterPVPBoost;
                            }
                        }

                        damageCounter += modDamage;

                        hits++;
                    }

                    if (base.isAuthority)
                    {
                        EffectData effectData = new EffectData();
                        effectData.rotation = Util.QuaternionSafeLookRotation(Vector3.zero);
                        effectData.origin = this.characterBody.corePosition;

                        EffectManager.SpawnEffect(Assets.blockEffect, effectData, true);
                    }
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            base.characterMotor.velocity = Vector3.zero;

            if (base.fixedAge >= this.invulEnd && invul)
            {
                if (NetworkServer.active && base.characterBody.HasBuff(RoR2Content.Buffs.HiddenInvincibility))
                {
                    base.characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
                }
                RemoveOverlay();
                invul = false;

                CapsuleCollider col = (CapsuleCollider)base.characterBody.mainHurtBox.collider;
                col.height = 1.5f;
                col.radius = 0.2f;

                // Counter Time
                if (damageCounter > 0f)
                {
                    if (base.isAuthority)
                    {
                        this.outer.SetNextState(new BlockCounter
                        {
                            damageCounter = this.damageCounter,
                            hits = this.hits,
                            bonusMult = this.bonusMult
                        });
                    }
                }
            }

            if (this.damageCounter > 0f && !base.inputBank.skill3.down && base.isAuthority)
            {
                this.outer.SetNextState(new BlockCounter
                {
                    damageCounter = this.damageCounter,
                    hits = this.hits,
                    bonusMult = this.bonusMult
                });
            }

            if (base.fixedAge >= this.invulEnd && !blockOut)
            {
                blockOut = true;
                base.PlayAnimation("FullBody, Override", "BlockOut");
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
            RemoveOverlay();

            CapsuleCollider col = (CapsuleCollider)base.characterBody.mainHurtBox.collider;
            col.height = 1.5f;
            col.radius = 0.2f;

            RiskOfRuinaNetworkManager.ServerOnHit -= OnHit;

            if (!blockOut)
            {
                base.PlayAnimation("FullBody, Override", "BlockOut");
            }

            base.OnExit();
        }

        protected void AddOverlay(float duration)
        {
            if (Config.iframeOverlay.Value)
            {
                iframeOverlay = base.characterBody.gameObject.AddComponent<TemporaryOverlay>();
                iframeOverlay.duration = duration;
                iframeOverlay.alphaCurve = AnimationCurve.Constant(0f, duration, 0.1f);
                iframeOverlay.animateShaderAlpha = true;
                iframeOverlay.destroyComponentOnEnd = true;
                iframeOverlay.originalMaterial = Resources.Load<Material>("Materials/matHuntressFlashBright");
                iframeOverlay.AddToCharacerModel(base.modelLocator.modelTransform.GetComponent<CharacterModel>());
            }
        }

        protected void RemoveOverlay()
        {
            if (iframeOverlay)
            {
                UnityEngine.Object.Destroy(iframeOverlay);
            }
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(this.damageCounter);
            writer.Write(this.hits);
            writer.Write(this.invulEnd);
            writer.Write(this.duration);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            this.damageCounter = reader.ReadSingle();
            this.hits = reader.ReadInt32();
            this.invulEnd = reader.ReadSingle();
            this.duration = reader.ReadSingle();
        }
    }
}
