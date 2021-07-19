using UnityEngine;

namespace RiskOfRuinaMod.SkillStates
{
    public class ChargePillarSpear : SkillStates.BaseStates.BaseChargeSpellState
    {
        private GameObject chargeEffect;
        private Vector3 originalScale;

        public override void OnEnter()
        {
            this.baseDuration = Modules.StaticValues.pillarSpearChargeDuration;
            this.chargeEffectPrefab = null;
            this.chargeSoundString = "Play_Binah_Stone_Ready";
            this.crosshairOverridePrefab = Resources.Load<GameObject>("Prefabs/Crosshair/ToolbotGrenadeLauncherCrosshair");
            this.maxBloomRadius = 0.1f;
            this.minBloomRadius = 1;

            base.OnEnter();

            ChildLocator childLocator = base.GetModelChildLocator();
            if (childLocator)
            {
                this.chargeEffect = childLocator.FindChild("SpearSummon").gameObject;
                this.chargeEffect.SetActive(true);
                this.originalScale = this.chargeEffect.transform.localScale;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            this.chargeEffect.transform.localScale = this.originalScale * (1 + this.CalcCharge());
        }

        public override void OnExit()
        {
            base.OnExit();

            if (this.chargeEffect)
            {
                this.chargeEffect.transform.localScale = this.originalScale;
                this.chargeEffect.SetActive(false);
            }
        }

        protected override SkillStates.BaseStates.BaseThrowSpellState GetNextState()
        {
            return new ThrowPillarSpear();
        }
    }
}
