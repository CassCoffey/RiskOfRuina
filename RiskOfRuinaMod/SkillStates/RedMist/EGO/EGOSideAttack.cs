using EntityStates.Mage;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskOfRuinaMod.SkillStates
{
    class EGOSideAttack : BaseStates.BaseDirectionalSkill
    {
        private float direction = 1f;

        public override void OnEnter()
        {
            if (this.attackIndex > 2)
            {
                this.attackIndex = 1;
            }

            if (inputVector.y > 0.5f)
            {
                direction = 1f;
            }
            else if (inputVector.y < -0.5f)
            {
                direction = -1f;
            }

            this.hitboxName = "EGOSide";

            this.damageCoefficient = Modules.StaticValues.sideAttackDamageCoefficient;
            this.baseDuration = 1.4f;
            this.attackStartTime = 0.35f;
            this.attackEndTime = 0.5f;
            this.baseEarlyExitTime = 0.8f;
            this.hitStopDuration = 0.05f;

            this.swingSoundString = "Ruina_Swipe";
            this.impactSound = Modules.Assets.swordHitEGOSoundHori.index;
            switch (attackIndex)
            {
                case (1):
                    this.muzzleString = "Side1";
                    break;
                case (2):
                    this.muzzleString = "Side2";
                    break;
            }
            this.hitEffectPrefab = Modules.Assets.swordHitEffect;

            if (NetworkServer.active)
            {
                base.characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
            }

            AddOverlay(baseDuration * attackStartTime);

            base.OnEnter();

            this.swingEffectPrefab = statTracker.EGOSlashPrefab;
        }

        protected override void PlayAttackAnimation()
        {
            base.PlayCrossfade("FullBody, Override", "EGOSideSlash" + attackIndex, "BaseAttack.playbackRate", this.duration, 0.1f);
        }

        protected override void PlaySwingEffect()
        {
            base.PlaySwingEffect();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!this.hasFired && !inHitPause)
            {
                Vector3 flatAimDirection = base.inputBank.aimDirection;
                flatAimDirection.y = 0;
                flatAimDirection.Normalize();

                float num = Mathf.Clamp(0f, 3.5f, 0.5f * trueMoveSpeed);
                Vector3 right = Vector3.Cross(Vector3.up, flatAimDirection).normalized;
                base.characterMotor.rootMotion += direction * (right * (num * FlyUpState.speedCoefficientCurve.Evaluate(base.fixedAge / (this.duration * this.attackEndTime)) * Time.fixedDeltaTime));
            }
        }

        protected override void OnHitEnemyAuthority()
        {
            base.OnHitEnemyAuthority();
        }

        protected override void FireAttack()
        {
            if (!this.hasFired && NetworkServer.active)
            {
                base.characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
            }
            RemoveOverlay();

            base.FireAttack();
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
    }
}
