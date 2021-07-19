using EntityStates;
using RoR2;
using RoR2.Audio;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskOfRuinaMod.SkillStates
{
    public class BasicStringStart : BaseStates.BaseDirectionalSkill
    {
        public override void OnEnter()
        {
            base.OnEnter();
            attackIndex = 0;
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        protected override void FireAttack()
        {
            // do nothing
        }

        public override void FixedUpdate()
        {
            if (base.isAuthority)
            {
                EvaluateInput();
                SetNextState();
            }
        }

        protected override void SetNextState()
        {
            base.SetNextState();
        }
    }
}