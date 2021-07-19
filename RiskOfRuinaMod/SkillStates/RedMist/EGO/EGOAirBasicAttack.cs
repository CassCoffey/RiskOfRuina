using EntityStates.Mage;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RiskOfRuinaMod.SkillStates
{
    class EGOAirBasicAttack : BaseStates.BaseDirectionalSkill
    {
        public override void OnEnter()
        {
            if (this.attackIndex > 2)
            {
                this.attackIndex = 1;
            }

            this.hitboxName = "EGOAirBasic";

            this.damageCoefficient = Modules.StaticValues.airBasicAttackDamageCoefficient;
            this.baseDuration = 1.0f;
            this.attackStartTime = 0.2f;
            this.attackEndTime = 0.4f;
            this.baseEarlyExitTime = 0.6f;
            this.hitStopDuration = 0.05f;
            this.swingHopVelocity = 6.0f;
            this.pushForce = 0f;
            this.bonusForce = Vector3.zero;

            this.swingSoundString = "Ruina_Swipe";
            this.impactSound = Modules.Assets.swordHitEGOSoundHori.index;
            switch (attackIndex)
            {
                case (1):
                    this.muzzleString = "Air1";
                    break;
                case (2):
                    this.muzzleString = "Air2";
                    break;
            }
            this.hitEffectPrefab = Modules.Assets.swordHitEffect;

            base.OnEnter();

            this.swingEffectPrefab = statTracker.EGOSlashPrefab;
        }

        protected override void PlayAttackAnimation()
        {
            base.PlayCrossfade("FullBody, Override", "EGOAirBasicSlash" + attackIndex, "BaseAttack.playbackRate", this.duration, 0.1f);
        }

        protected override void PlaySwingEffect()
        {
            base.PlaySwingEffect();
        }

        protected override void OnHitEnemyAuthority()
        {
            base.OnHitEnemyAuthority();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        protected override void FireAttack()
        {
            base.FireAttack();

            if (inputVector != Vector2.zero)
            {
                float num = Mathf.Clamp(0f, 2f, 0.5f * trueMoveSpeed);
                base.characterMotor.rootMotion += inputBank.moveVector * (num * FlyUpState.speedCoefficientCurve.Evaluate(base.fixedAge / this.duration) * Time.fixedDeltaTime);
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
