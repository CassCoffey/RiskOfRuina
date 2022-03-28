using RiskOfRuinaMod.Modules;
using R2API;
using RoR2;
using UnityEngine;

namespace RiskOfRuinaMod.SkillStates
{
    public class ScepterCastShockwave : BaseStates.BaseCastChanneledSpellState
    {
        private Vector3 storedPosition;

        private ShakeEmitter shakeEmitter;

        public override void OnEnter()
        {
            this.baseDuration = 0.75f;
            this.baseInterval = 0f;
            this.centered = true;
            this.muzzleString = "HandR";
            this.muzzleflashEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/CrocoDiseaseImpactEffect");
            this.projectilePrefabs.Enqueue(Projectiles.shockwaveScepterPrefab);
            this.castSoundString = "Play_Abiter_Special_Boom";

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

            shakeEmitter = base.gameObject.AddComponent<ShakeEmitter>();
            shakeEmitter.amplitudeTimeDecay = true;
            shakeEmitter.duration = 1.5f;
            shakeEmitter.radius = 200f;
            shakeEmitter.scaleShakeRadiusWithLocalScale = false;

            shakeEmitter.wave = new Wave
            {
                amplitude = 0.2f,
                frequency = 20f,
                cycleOffset = 0f
            };

            float radius = StaticValues.shockwaveScepterRadius;

            float damageCo = StaticValues.shockwaveScepterDamageCoefficient;

            if (base.isAuthority)
            {
                BlastAttack blastAttack = new BlastAttack();
                blastAttack.attacker = base.gameObject;
                blastAttack.inflictor = base.gameObject;
                blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
                blastAttack.position = base.transform.position;
                blastAttack.procCoefficient = 0.5f;
                blastAttack.radius = radius;
                blastAttack.baseForce = 8000f;
                blastAttack.bonusForce = Vector3.zero;
                blastAttack.baseDamage = damageCo * this.damageStat;
                blastAttack.falloffModel = BlastAttack.FalloffModel.None;
                blastAttack.damageColorIndex = DamageColorIndex.Default;
                blastAttack.attackerFiltering = AttackerFiltering.NeverHitSelf;
                blastAttack.crit = base.RollCrit();
                blastAttack.damageType = DamageType.Stun1s;
                blastAttack.Fire();
            }

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
    }
}
