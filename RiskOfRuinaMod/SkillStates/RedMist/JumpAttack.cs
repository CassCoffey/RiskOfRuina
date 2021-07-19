using EntityStates.Mage;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RiskOfRuinaMod.SkillStates
{
    class JumpAttack : BaseStates.BaseDirectionalSkill
    {
        public override void OnEnter()
        {
            this.attackIndex = 1;

            this.hitboxName = "Jump";

            this.damageCoefficient = Modules.StaticValues.jumpAttackDamageCoefficient;
            this.baseDuration = 1f;
            this.attackStartTime = 0.35f;
            this.attackEndTime = 0.5f;
            this.baseEarlyExitTime = 0.3f;
            this.hitStopDuration = 0.05f;

            this.swingSoundString = "Ruina_Swipe";
            this.impactSound = Modules.Assets.swordHitSoundVert.index;
            this.muzzleString = "Jump";
            this.hitEffectPrefab = Modules.Assets.swordHitEffect;

            this.bonusForce = Vector3.up * 3000;

            base.OnEnter();

            this.swingEffectPrefab = statTracker.slashPrefab;
        }

        protected override void PlayAttackAnimation()
        {
            base.PlayCrossfade("FullBody, Override", "JumpSlash", "BaseAttack.playbackRate", this.duration, 0.1f);
        }

        protected override void PlaySwingEffect()
        {
            base.PlaySwingEffect();
        }

        protected override void OnHitEnemyAuthority()
        {
            base.OnHitEnemyAuthority();
        }

        protected override void FireAttack()
        {
            base.FireAttack();

            //float num = Mathf.Clamp(0f, 1f, 0.5f * trueMoveSpeed);
            //base.characterMotor.rootMotion -= base.characterDirection.forward * (num * FlyUpState.speedCoefficientCurve.Evaluate(base.fixedAge / this.duration) * Time.fixedDeltaTime);

            if (base.inputBank.skill1.down && base.inputBank.jump.down)
            {
                // You held it down, you're insane
                this.outer.SetNextState(new JumpRisingAttack
                {
                    attackIndex = attackIndex,
                    inputVector = inputVector
                });
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
