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
    class Dodge : BaseSkillState
    {
        public Vector3 dodgeVector;
        public float duration = 0.65f;
        public float moveEnd = 0.65f;
        public float invulStart = 0f;
        public float invulEnd = 0.4f;
        public float stockBonus = 0.05f;
        public bool invul = false;
        public bool aerial = false;

        protected TemporaryOverlay iframeOverlay;

        protected float trueMoveSpeed
        {
            get { return this.GetComponent<RedMistStatTracker>().modifiedMoveSpeed; }
        }

        protected float trueAttackSpeed
        {
            get { return this.GetComponent<RedMistStatTracker>().modifiedAttackSpeed; }
        }

        protected float trueDamage
        {
            get { return this.damageStat + (this.GetComponent<RedMistStatTracker>().DifferenceAttackSpeed * 10f) + (this.GetComponent<RedMistStatTracker>().DifferenceMoveSpeed); }
        }


        public override void OnEnter()
        {
            this.dodgeVector = inputBank.moveVector;
            if (!this.characterMotor.isGrounded)
            {
                aerial = true;
                base.SmallHop(this.characterMotor, 10f);
            }

            if (base.skillLocator.utility.stock > 1)
            {
                this.invulEnd += (base.skillLocator.utility.stock - 1) * this.stockBonus;
            }

            AddOverlay(invulEnd);

            base.OnEnter();

            Util.PlaySound("Ruina_Swipe", base.gameObject);
            base.PlayCrossfade("FullBody, Override", "Dodge", "Dodge.playbackRate", this.duration, 0.1f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!aerial)
            {
                if (base.fixedAge <= this.moveEnd)
                {
                    base.characterMotor.rootMotion += dodgeVector * (3.5f * FlyUpState.speedCoefficientCurve.Evaluate(base.fixedAge / (this.moveEnd * 1.3f)) * Time.fixedDeltaTime);
                    base.characterMotor.velocity.y = 0f;
                    base.characterMotor.moveDirection *= 2f;
                }
            } else
            {
                if (base.fixedAge <= this.moveEnd)
                {
                    base.characterMotor.rootMotion += dodgeVector * (2f * FlyUpState.speedCoefficientCurve.Evaluate(base.fixedAge / (this.moveEnd * 1.3f)) * Time.fixedDeltaTime);
                    base.characterMotor.moveDirection *= 2f;
                }
            }

            if (NetworkServer.active && base.fixedAge >= this.invulStart && !invul)
            {
                base.characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
                invul = true;
            }

            if (NetworkServer.active && base.fixedAge >= this.invulEnd && invul)
            {
                if (base.characterBody.HasBuff(RoR2Content.Buffs.HiddenInvincibility))
                {
                    base.characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
                }
                RemoveOverlay();
                invul = false;
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
    }
}
