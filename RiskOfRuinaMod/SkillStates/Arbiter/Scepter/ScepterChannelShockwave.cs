using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RiskOfRuinaMod.Modules;
using RoR2;

namespace RiskOfRuinaMod.SkillStates
{
    class ScepterChannelShockwave : BaseStates.BaseChannelSpellState
    {
        private GameObject chargeEffect;

        private ShakeEmitter shakeEmitter;

        public override void OnEnter()
        {
            this.chargeEffectPrefab = null;
            this.startChargeSoundString = "Play_Abiter_Special_Start";
            this.maxSpellRadius = Modules.StaticValues.shockwaveScepterRadius;
            this.baseDuration = StaticValues.shockwaveScepterChannelDuration;
            this.zooming = true;
            this.centered = true;

            shakeEmitter = base.gameObject.AddComponent<ShakeEmitter>();
            shakeEmitter.amplitudeTimeDecay = false;
            shakeEmitter.duration = this.baseDuration / (this.attackSpeedStat / 2f);
            shakeEmitter.radius = 60f;
            shakeEmitter.scaleShakeRadiusWithLocalScale = false;

            shakeEmitter.wave = new Wave
            {
                amplitude = 0.05f,
                frequency = 15f,
                cycleOffset = 0f
            };

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
            RiskOfRuinaPlugin.Destroy(shakeEmitter);

            base.OnExit();
        }

        protected override BaseStates.BaseCastChanneledSpellState GetNextState()
        {
            return new ScepterCastShockwave();
        }
    }
}
