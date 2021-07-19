using EntityStates.Mage;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskOfRuinaMod.SkillStates
{
    class AirBackAttack : BaseStates.BaseDirectionalSkill
    {
        public override void OnEnter()
        {
            this.attackIndex = 1;

            this.hitboxName = "AirBasic";

            this.damageCoefficient = Modules.StaticValues.airBackAttackDamageCoefficient;
            this.baseDuration = 0.25f;
            this.attackStartTime = 0.5f;
            this.attackEndTime = 0.8f;
            this.baseEarlyExitTime = 0f;
            this.hitStopDuration = 0.05f;

            this.swingSoundString = "Ruina_Swipe";
            this.impactSound = Modules.Assets.swordHitSoundVert.index;
            this.muzzleString = "SwingLeft";
            this.hitEffectPrefab = Modules.Assets.swordHitEffect;

            this.bonusForce = Vector3.down * 900;

            base.OnEnter();

            this.swingEffectPrefab = statTracker.slashPrefab;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            this.characterMotor.velocity.y = 0;

            if (this.stopwatch > (this.duration * this.attackEndTime))
            {
                this.outer.SetNextState(new AirBackFallingAttack());
            }
        }

        protected override void PlayAttackAnimation()
        {
            base.PlayCrossfade("FullBody, Override", "AirBackSlash", "BaseAttack.playbackRate", this.duration, 0.1f);
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

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
