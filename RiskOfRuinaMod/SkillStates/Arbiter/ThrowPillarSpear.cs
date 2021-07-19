using RiskOfRuinaMod.Modules;

namespace RiskOfRuinaMod.SkillStates
{
    public class ThrowPillarSpear : SkillStates.BaseStates.BaseThrowSpellState
    {
        public override void OnEnter()
        {
            this.baseDuration = 0.8f;
            this.force = 5f;
            this.maxDamageCoefficient = StaticValues.pillarSpearMaxDamageCoefficient;
            this.minDamageCoefficient = StaticValues.pillarSpearMinDamageCoefficient;
            this.muzzleflashEffectPrefab = EntityStates.Commando.CommandoWeapon.FirePistol2.muzzleEffectPrefab;
            this.projectilePrefab = Modules.Projectiles.pillarSpearPrefab;
            this.selfForce = 0f;
            this.throwSound = "Play_Binah_Stone_Fire";

            base.OnEnter();
        }
    }
}
