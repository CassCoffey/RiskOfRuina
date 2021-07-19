using EntityStates.Mage;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskOfRuinaMod.SkillStates
{
    class EGOForwardAttack : BaseStates.BaseDirectionalSkill
    {
        public override void OnEnter()
        {
            if (this.attackIndex > 4)
            {
                this.attackIndex = 2;
            }

            this.hitboxName = "EGOForward";

            this.damageCoefficient = Modules.StaticValues.forwardAttackDamageCoefficient;
            this.damageType = DamageType.BypassArmor;
            this.baseDuration = 1.4f;
            this.attackStartTime = 0.2f;
            this.attackEndTime = 0.4f;
            this.baseEarlyExitTime = 1.0f;
            this.hitStopDuration = 0.05f;

            if (this.attackIndex >= 2)
            {
                this.baseDuration = 0.9f;
                this.attackStartTime = 0.1f;
                this.attackEndTime = 0.3f;
                this.baseEarlyExitTime = 0.65f;
                this.hitStopDuration = 0.05f;
                this.damageCoefficient = Modules.StaticValues.forwardAttackComboDamageCoefficient;
            }

            this.swingSoundString = "Ruina_Swipe";
            this.impactSound = Modules.Assets.swordHitEGOSoundStab.index;
            switch (attackIndex)
            {
                case (1):
                    this.muzzleString = "Spear1";
                    break;
                case (2):
                    this.muzzleString = "Spear1";
                    break;
                case (3):
                    this.muzzleString = "EGOSpear3";
                    break;
                case (4):
                    this.muzzleString = "EGOSpear4";
                    break;
            }
            this.hitEffectPrefab = Modules.Assets.swordHitEffect;

            if (this.attackIndex == 1)
            {
                if (NetworkServer.active)
                {
                    base.characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
                }
                AddOverlay(baseDuration * attackStartTime);
            }

            base.OnEnter();

            this.swingEffectPrefab = statTracker.EGOPiercePrefab;
        }

        protected override void PlayAttackAnimation()
        {
            if (this.attackIndex >= 2)
            {
                base.PlayCrossfade("FullBody, Override", "EGOForwardSpear" + this.attackIndex, "BaseAttack.playbackRate", this.duration, 0.01f);
            } else
            {
                base.PlayCrossfade("FullBody, Override", "EGOForwardSpear1", "BaseAttack.playbackRate", this.duration, 0.1f);
            }
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
            if (!this.hasFired && this.attackIndex == 1)
            {
                if (NetworkServer.active)
                {
                    base.characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
                }
                RemoveOverlay();
            }

            base.FireAttack();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!this.hasFired && !inHitPause)
            {
                Vector3 flatAimDirection = base.inputBank.aimDirection;
                flatAimDirection.y = 0;
                flatAimDirection.Normalize();

                if (this.attackIndex >= 2)
                {
                    float num = 0.25f;
                    base.characterMotor.rootMotion += flatAimDirection * (num * FlyUpState.speedCoefficientCurve.Evaluate(base.fixedAge / (this.duration * this.attackEndTime)) * Time.fixedDeltaTime);
                }
                else
                {
                    float num = 9f;
                    base.characterMotor.rootMotion += flatAimDirection * (num * FlyUpState.speedCoefficientCurve.Evaluate(base.fixedAge / (this.duration * this.attackEndTime)) * Time.fixedDeltaTime);
                }
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
    }
}
