using JetBrains.Annotations;
using RoR2;
using RoR2.Skills;

namespace RiskOfRuinaMod.Modules.Components
{
    class TargettedSkillDef : SkillDef
    {
        public float cost;

        public override SkillDef.BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
        {
            return new TargettedSkillDef.InstanceData
            {
                TrackerComponent = skillSlot.GetComponent<TargetTracker>()
            };
        }

        private static bool HasTarget([NotNull] GenericSkill skillSlot)
        {
            TargetTracker Tracker = ((TargettedSkillDef.InstanceData)skillSlot.skillInstanceData).TrackerComponent;
            return (Tracker != null) ? (Tracker.GetTrackingTarget()) : false;
        }

        public override bool CanExecute([NotNull] GenericSkill skillSlot)
        {
            return TargettedSkillDef.HasTarget(skillSlot) && base.CanExecute(skillSlot);
        }

        public override bool IsReady([NotNull] GenericSkill skillSlot)
        {
            return base.IsReady(skillSlot) && TargettedSkillDef.HasTarget(skillSlot);
        }

        protected class InstanceData : SkillDef.BaseSkillInstanceData
        {
            public TargetTracker TrackerComponent;
        }
    }
}
