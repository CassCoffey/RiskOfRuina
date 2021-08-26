using RiskOfRuinaMod.SkillStates.BaseStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace RiskOfRuinaMod.SkillStates
{
    public class FairyCombo : BaseMeleeAttack
    {
        private bool firedProjectile = false;

        public override void OnEnter()
        {
            this.hitboxName = "Fairy";

            this.damageType = DamageType.Generic;
            this.damageCoefficient = Modules.StaticValues.fairyDamageCoefficient;
            this.procCoefficient = 1f;
            this.pushForce = 300f;
            this.bonusForce = Vector3.zero;
            this.baseDuration = 1f;
            this.attackStartTime = 0.2f;
            this.attackEndTime = 0.4f;
            this.baseEarlyExitTime = 0.2f;
            this.hitStopDuration = 0.012f;
            this.attackRecoil = 0.5f;
            this.hitHopVelocity = 4f;

            this.swingSoundString = "Ruina_Swipe";
            this.impactSound = RoR2.Audio.NetworkSoundEventIndex.Invalid;
            this.muzzleString = swingIndex % 2 == 0 ? "SwingLeft" : "SwingRight";
            this.swingEffectPrefab = Modules.Assets.armSwingEffect;
            this.hitEffectPrefab = Modules.Assets.fairyHitEffect;

            base.OnEnter();
        }

        protected override void PlayAttackAnimation()
        {
            base.PlayCrossfade("Gesture, Override", "Swipe" + (1 + swingIndex), "Swipe.playbackRate", this.duration * 0.6f, 0.05f);
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

            if (base.isAuthority && !firedProjectile)
            {
                firedProjectile = true;
                Ray aimRay = base.GetAimRay();

                if (Modules.Projectiles.fairyLinePrefab != null)
                {
                    FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
                    {
                        projectilePrefab = Modules.Projectiles.fairyLinePrefab,
                        position = aimRay.origin,
                        rotation = Util.QuaternionSafeLookRotation(aimRay.direction),
                        owner = base.gameObject,
                        damage = this.damageStat * Modules.StaticValues.fairyDamageCoefficient,
                        force = 0f,
                        crit = base.RollCrit(),
                        speedOverride = 150f
                    };

                    ProjectileManager.instance.FireProjectile(fireProjectileInfo);
                }
            }
        }

        protected override void SetNextState()
        {
            int index = this.swingIndex;
            index++;
            if (index > 2)
            {
                this.outer.SetNextStateToMain();
            } else
            {
                this.outer.SetNextState(new FairyCombo
                {
                    swingIndex = index
                });
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}