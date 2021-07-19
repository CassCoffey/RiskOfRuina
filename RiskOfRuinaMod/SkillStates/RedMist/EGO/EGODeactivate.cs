using EntityStates;
using RiskOfRuinaMod.Modules;
using RiskOfRuinaMod.Modules.Components;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
namespace RiskOfRuinaMod.SkillStates
{
    class EGODeactivate : BaseSkillState
    {
        public static float baseDuration = 1f;

        private float duration;
        private RedMistEmotionComponent EGOController;

        public override void OnEnter()
        {
            base.OnEnter();
            this.EGOController = this.gameObject.GetComponent<RedMistEmotionComponent>();

            Util.PlaySound("Play_Effect_Break", base.gameObject);
            EffectData effectData = new EffectData();
            effectData.origin = base.characterBody.footPosition;
            effectData.scale = 1;
            EffectManager.SpawnEffect(Assets.EGODeactivate, effectData, false);

            base.PlayAnimation("FullBody, Override", "BufferEmpty");

            if (base.skillLocator.utility.baseSkill == Modules.Survivors.RedMist.NormalBlock)
            {
                base.skillLocator.utility.UnsetSkillOverride(base.skillLocator.utility, Modules.Survivors.RedMist.EGOBlock, GenericSkill.SkillOverridePriority.Contextual);
            }
            else if (base.skillLocator.utility.baseSkill == Modules.Survivors.RedMist.NormalDodge)
            {
                base.skillLocator.utility.UnsetSkillOverride(base.skillLocator.utility, Modules.Survivors.RedMist.EGODodge, GenericSkill.SkillOverridePriority.Contextual);
            }

            base.skillLocator.special.UnsetSkillOverride(base.skillLocator.special, Modules.Survivors.RedMist.HorizontalSlash, GenericSkill.SkillOverridePriority.Contextual);

            this.outer.SetNextStateToMain();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}
