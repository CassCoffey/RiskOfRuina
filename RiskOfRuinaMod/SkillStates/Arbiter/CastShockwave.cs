using RiskOfRuinaMod.Modules;
using EntityStates;
using R2API;
using RoR2;
using UnityEngine;

namespace RiskOfRuinaMod.SkillStates
{
    public class CastShockwave : BaseStates.BaseCastChanneledSpellState
    {
        private Vector3 storedPosition;
        private int shockwaveNum = 0;

        public override void OnEnter()
        {
            this.baseDuration = 3.5f;
            this.baseInterval = 1.5f;
            this.centered = true;
            this.muzzleString = "HandR";
            this.muzzleflashEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/CrocoDiseaseImpactEffect");
            this.projectilePrefabs.Enqueue(Projectiles.shockwaveSmallPrefab);
            this.projectilePrefabs.Enqueue(Projectiles.shockwaveMediumPrefab);
            this.projectilePrefabs.Enqueue(Projectiles.shockwaveLargePrefab);
            this.castSoundString = "Play_Binah_Shockwave";

            this.storedPosition = base.transform.position;

            base.OnEnter();
        }

        protected override void PlayCastAnimation()
        {
            base.PlayAnimation("Gesture, Override", "CastShockwave", "Shockwave.playbackRate", 0.25f);
        }

        protected override void Fire()
        {
            if (projectilePrefabs.Count <= 0) return;

            float radius = StaticValues.shockwaveMinRadius;
            if (shockwaveNum == 1) radius = StaticValues.shockwaveMinRadius + ((StaticValues.shockwaveMaxRadius - StaticValues.shockwaveMinRadius) / 2f);
            if (shockwaveNum == 2) radius = StaticValues.shockwaveMaxRadius;

            float damageCo = StaticValues.shockwaveMinDamageCoefficient;
            if (shockwaveNum == 1) damageCo = StaticValues.shockwaveMinDamageCoefficient + ((StaticValues.shockwaveMaxDamageCoefficient - StaticValues.shockwaveMinDamageCoefficient) / 2f);
            if (shockwaveNum == 2) damageCo = StaticValues.shockwaveMaxDamageCoefficient;

            if (base.isAuthority)
            {
                BlastAttack blastAttack = new BlastAttack();
                blastAttack.attacker = base.gameObject;
                blastAttack.inflictor = base.gameObject;
                blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
                blastAttack.position = base.transform.position;
                blastAttack.procCoefficient = 1f;
                blastAttack.radius = radius;
                blastAttack.baseForce = 2000f;
                blastAttack.bonusForce = Vector3.zero;
                blastAttack.baseDamage = damageCo * this.damageStat;
                blastAttack.falloffModel = BlastAttack.FalloffModel.None;
                blastAttack.damageColorIndex = DamageColorIndex.Default;
                blastAttack.attackerFiltering = AttackerFiltering.NeverHit;
                blastAttack.crit = base.RollCrit();
                blastAttack.damageType = DamageType.Stun1s;
                blastAttack.Fire();
            }

            shockwaveNum++;

            base.Fire();
        }

        public override void OnExit()
        {
            base.OnExit();

            base.PlayAnimation("Gesture, Override", "CastShockwaveEnd", "Shockwave.playbackRate", 0.8f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            base.transform.position = this.storedPosition;
            bool flag = base.characterMotor;
            if (flag)
            {
                base.characterMotor.velocity = Vector3.zero;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}
