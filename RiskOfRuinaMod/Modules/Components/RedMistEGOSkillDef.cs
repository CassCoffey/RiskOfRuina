using JetBrains.Annotations;
using RoR2;
using RoR2.Skills;

namespace RiskOfRuinaMod.Modules.Components
{
    class RedMistEGOSkillDef : SkillDef
    {
        public float cost;

        public override SkillDef.BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
        {
            return new RedMistEGOSkillDef.InstanceData
            {
                EGOComponent = skillSlot.GetComponent<RedMistEmotionComponent>()
            };
        }

        private static bool HasSufficientEGO([NotNull] GenericSkill skillSlot)
        {
            RedMistEmotionComponent EGOComponent = ((RedMistEGOSkillDef.InstanceData)skillSlot.skillInstanceData).EGOComponent;
            return (EGOComponent != null) ? (EGOComponent.currentEmotion >= skillSlot.rechargeStock) : false;
        }

        public override bool CanExecute([NotNull] GenericSkill skillSlot)
        {
            return RedMistEGOSkillDef.HasSufficientEGO(skillSlot) && base.CanExecute(skillSlot);
        }

        public override bool IsReady([NotNull] GenericSkill skillSlot)
        {
            return base.IsReady(skillSlot) && RedMistEGOSkillDef.HasSufficientEGO(skillSlot);
        }

        protected class InstanceData : SkillDef.BaseSkillInstanceData
        {
            public RedMistEmotionComponent EGOComponent;
        }
    }
}
