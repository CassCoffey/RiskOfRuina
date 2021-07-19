using EntityStates;
using EntityStates.Mage;
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
    class JumpRisingAttack : BaseSkillState
    {
        public int attackIndex = 1;
        public Vector2 inputVector;
        public float duration;
        protected string swingSoundString = "";
        protected string hitSoundString = "";
        protected string muzzleString = "SwingCenter";
        protected string attackAnimation = "Swing";
        protected GameObject swingEffectPrefab;
        protected GameObject hitEffectPrefab;

        protected float trueMoveSpeed
        {
            get { return this.GetComponent<RedMistStatTracker>().modifiedMoveSpeed; }
        }


        public override void OnEnter()
        {
            this.attackIndex = 1;

            this.duration = 0.4f;

            this.swingSoundString = "Ruina_Swipe";
            this.hitSoundString = "Fairy";
            this.muzzleString = "SwingLeft";
            this.swingEffectPrefab = Modules.Assets.swordSwingEffect;
            this.hitEffectPrefab = Modules.Assets.swordHitEffect;

            base.OnEnter();

            if (NetworkServer.active)
            {
                base.characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
            }

            this.PlayAttackAnimation();
        }

        protected void PlayAttackAnimation()
        {
            base.PlayCrossfade("FullBody, Override", "JumpSlashContinue", "BaseAttack.playbackRate", this.duration, 0.1f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            float num = Mathf.Clamp(0f, 10f, 0.5f * this.trueMoveSpeed);
            base.characterMotor.rootMotion += Vector3.up * (num * FlyUpState.speedCoefficientCurve.Evaluate(base.fixedAge / this.duration) * Time.fixedDeltaTime);
            base.characterMotor.velocity.y = 0f;
            base.characterMotor.moveDirection *= 2f;
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

            base.OnExit();
        }
    }
}
