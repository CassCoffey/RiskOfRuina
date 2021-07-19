using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RiskOfRuinaMod.Modules;

namespace RiskOfRuinaMod.SkillStates
{
    class ScepterChannelShockwave : BaseStates.BaseChannelSpellState
    {
        private GameObject chargeEffect;

        public override void OnEnter()
        {
            this.chargeEffectPrefab = null;
            this.startChargeSoundString = "Play_Abiter_Special_Start";
            this.maxSpellRadius = Modules.StaticValues.shockwaveScepterRadius;
            this.baseDuration = 0.25f * StaticValues.shockwaveChannelDuration;
            this.zooming = false;
            this.centered = true;

            base.OnEnter();
        }

        protected override void PlayChannelAnimation()
        {
            base.PlayAnimation("Gesture, Override", "Channel", "Channel.playbackRate", this.baseDuration);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        protected override BaseStates.BaseCastChanneledSpellState GetNextState()
        {
            return new ScepterCastShockwave();
        }
    }
}
