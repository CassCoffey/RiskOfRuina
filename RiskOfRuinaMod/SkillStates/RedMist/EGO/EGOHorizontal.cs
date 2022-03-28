using EntityStates;
using EntityStates.Mage;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskOfRuinaMod.SkillStates
{
    class EGOHorizontal : BaseStates.BaseDirectionalSkill
    {
        private float hopEndTime = 0.5f;
        private float moveEndtime = 1.0f;

        private bool aerial = false;

        private float originalTurnSpeed;
        private Vector3 savedAimDir;
        private bool hasAimDir;

        public ShakeEmitter shakeEmitter;

        private CameraTargetParams.AimRequest aimRequest;

        public override void OnEnter()
        {
            this.attackIndex = 1;

            this.hitboxName = "Horizontal";

            this.damageCoefficient = Modules.StaticValues.horizontalDamageCoefficient;
            this.baseDuration = 2.25f;
            this.attackStartTime = 0.45f;
            this.attackEndTime = 0.6f;
            this.baseEarlyExitTime = 0.5f;
            this.hitStopDuration = 0.05f;
            this.swingHopVelocity = 0f;
            this.bonusForce = this.characterDirection.forward * 4000 + Vector3.up * 2500;
            this.procCoefficient = 0.75f;

            this.swingSoundString = "Play_Kali_Special_Hori_Start";
            this.impactSound = Modules.Assets.swordHitEGOSoundGRHorizontal.index;
            this.muzzleString = "Horizontal";
            this.hitEffectPrefab = Modules.Assets.swordHitEffect;

            aerial = !characterMotor.isGrounded;

            if (NetworkServer.active)
            {
                base.characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
            }

            base.OnEnter();

            originalTurnSpeed = base.characterDirection.turnSpeed;
            this.swingEffectPrefab = statTracker.EGOHorizontalPrefab;

            base.cameraTargetParams.cameraParams = Modules.CameraParams.HorizontalSlashCameraParamsRedMist;
            aimRequest = base.cameraTargetParams.RequestAimType(CameraTargetParams.AimType.Aura);
        }

        protected override void PlayAttackAnimation()
        {
            base.PlayCrossfade("FullBody, Override", "HorizontalSlash", "BaseAttack.playbackRate", this.duration, 0.1f);
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

            if (this.stopwatch <= this.hopEndTime)
            {
                float num = 5f;
                if (aerial) num = 2f;
                base.characterMotor.rootMotion += Vector3.up * (num * FlyUpState.speedCoefficientCurve.Evaluate(base.stopwatch / this.hopEndTime) * Time.fixedDeltaTime);
            }
            if (this.stopwatch <= this.moveEndtime)
            {
                float num = 5f;
                base.characterMotor.rootMotion += inputBank.moveVector * (num * FlyUpState.speedCoefficientCurve.Evaluate(base.stopwatch / this.moveEndtime) * Time.fixedDeltaTime);
                base.characterMotor.moveDirection *= 2f;
                base.characterDirection.turnSpeed = 0f;
                base.characterDirection.forward = inputBank.aimDirection;
            }
            if (this.stopwatch > this.moveEndtime)
            {
                if (hasAimDir)
                {
                    base.characterDirection.forward = savedAimDir;
                } else
                {
                    savedAimDir = inputBank.aimDirection;
                    hasAimDir = true;
                }
            }

            base.characterMotor.velocity.y = 0f;
        }

        protected override void FireAttack()
        {
            base.FireAttack();

            shakeEmitter = base.gameObject.AddComponent<ShakeEmitter>();
            shakeEmitter.amplitudeTimeDecay = true;
            shakeEmitter.duration = 0.3f;
            shakeEmitter.radius = 100f;
            shakeEmitter.scaleShakeRadiusWithLocalScale = false;

            shakeEmitter.wave = new Wave
            {
                amplitude = 0.6f,
                frequency = 25f,
                cycleOffset = 0f
            };
        }

        public override void OnExit()
        {
            if (NetworkServer.active && base.characterBody.HasBuff(RoR2Content.Buffs.HiddenInvincibility))
            {
                base.characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
            }

            base.cameraTargetParams.cameraParams = Modules.CameraParams.defaultCameraParamsRedMist;
            aimRequest?.Dispose();

            base.characterDirection.turnSpeed = originalTurnSpeed;

            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}
