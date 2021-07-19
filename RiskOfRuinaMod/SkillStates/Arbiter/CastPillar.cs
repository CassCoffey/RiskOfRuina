using EntityStates;
using RoR2;
using UnityEngine;
using RiskOfRuinaMod.Modules.Components;
using R2API;
using RoR2.Projectile;

namespace RiskOfRuinaMod.SkillStates
{
    public class CastPillar : BaseStates.BaseCastChanneledSpellState
    {
        public override void OnEnter()
        {
            this.baseDuration = 0.5f;
            this.baseInterval = 0f;
            this.muzzleString = "HandR";
            this.muzzleflashEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/CrocoDiseaseImpactEffect");
            this.projectilePrefabs.Enqueue(Modules.Projectiles.pillarPrefab);
            this.castSoundString = "Play_Binah_Stone_Ready";

            base.OnEnter();
        }

        protected override void Fire()
        {
            if (projectilePrefabs.Count <= 0) return;

            if (base.isAuthority)
            {
                BlastAttack blastAttack = new BlastAttack();
                blastAttack.attacker = base.gameObject;
                blastAttack.inflictor = base.gameObject;
                blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
                blastAttack.position = this.spellPosition;
                blastAttack.procCoefficient = 1f;
                blastAttack.radius = Modules.StaticValues.pillarRadius / 2f;
                blastAttack.baseForce = 2000f;
                blastAttack.bonusForce = Vector3.zero;
                blastAttack.baseDamage = Modules.StaticValues.pillarDamageCoefficient * this.damageStat;
                blastAttack.falloffModel = BlastAttack.FalloffModel.SweetSpot;
                blastAttack.damageColorIndex = DamageColorIndex.Default;
                blastAttack.attackerFiltering = AttackerFiltering.NeverHit;
                blastAttack.crit = base.RollCrit();
                blastAttack.damageType = DamageType.AOE;
                blastAttack.Fire();
            }

            base.Fire();
        }

        protected override void PlayCastAnimation()
        {
            base.PlayAnimation("Gesture, Override", "Pillar", "Pillar.playbackRate", this.baseDuration);
        }
    }
}