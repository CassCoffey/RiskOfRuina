using EntityStates.Mage;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskOfRuinaMod.SkillStates
{
    class EGOBackAttack : BaseStates.BaseDirectionalSkill
    {
        public override void OnEnter()
        {
            this.attackIndex = 1;

            this.hitboxName = "EGOBack";

            this.damageCoefficient = Modules.StaticValues.backAttackDamageCoefficient;
            this.baseDuration = 1.0f;
            this.attackStartTime = 0.4f;
            this.attackEndTime = 0.6f;
            this.baseEarlyExitTime = 0.4f;
            this.hitStopDuration = 0.05f;

            this.swingSoundString = "Ruina_Swipe";
            this.impactSound = Modules.Assets.swordHitEGOSoundHori.index;
            this.muzzleString = "BasicSwing3";
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

            this.swingEffectPrefab = statTracker.EGOSlashPrefab;
        }

        protected override void PlayAttackAnimation()
        {
            base.PlayCrossfade("FullBody, Override", "EGOBackSlash", "BaseAttack.playbackRate", this.duration, 0.1f);
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
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (this.stopwatch <= (this.duration * this.attackEndTime) && !inHitPause)
            {
                Vector3 flatAimDirection = base.inputBank.aimDirection;
                flatAimDirection.y = 0;
                flatAimDirection.Normalize();
                float num = 7f;
                base.characterMotor.rootMotion -= flatAimDirection * (num * FlyUpState.speedCoefficientCurve.Evaluate(base.fixedAge / (this.duration * this.attackEndTime)) * Time.fixedDeltaTime);
            }

            if (this.stopwatch > (this.duration * this.attackStartTime))
            {
                if (NetworkServer.active && base.characterBody.HasBuff(RoR2Content.Buffs.HiddenInvincibility))
                {
                    base.characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
                }
                RemoveOverlay();
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
