using System;
using System.Collections.Generic;
using System.Text;

namespace RiskOfRuinaMod.SkillStates
{
    class ChannelPillar : BaseStates.BaseChannelSpellState
    {
        public override void OnEnter()
        {
            this.chargeEffectPrefab = null;
            this.maxSpellRadius = Modules.StaticValues.pillarRadius;
            this.baseDuration = 0.25f * Modules.StaticValues.pillarChannelDuration;
            this.zooming = false;
            this.line = true;

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
            return new CastPillar();
        }
    }
}
