using RiskOfRuinaMod.SkillStates;
using RiskOfRuinaMod.SkillStates.BaseStates;
using System.Collections.Generic;
using System;
using RiskOfRuinaMod.Modules.Misc;

namespace RiskOfRuinaMod.Modules
{
    public static class States
    {
        internal static List<Type> entityStates = new List<Type>();

        internal static void RegisterStates()
        {
            AddSkill(typeof(BaseDirectionalSkill));
            AddSkill(typeof(BaseMeleeAttack));
            AddSkill(typeof(BaseChargeSpellState));
            AddSkill(typeof(BaseChannelSpellState));
            AddSkill(typeof(BaseCastChanneledSpellState));


            // Red Mist
            AddSkill(typeof(EGOActivate));
            AddSkill(typeof(EGOActivateOut));
            AddSkill(typeof(EGODeactivate));
            AddSkill(typeof(EGOHorizontal));

            AddSkill(typeof(AirBackAttack));
            AddSkill(typeof(AirBackFallingAttack));
            AddSkill(typeof(AirBasicAttack));
            AddSkill(typeof(BackAttack));
            AddSkill(typeof(BasicAttack));
            AddSkill(typeof(BasicStringStart));
            AddSkill(typeof(Dodge));
            AddSkill(typeof(ForwardAttack));
            AddSkill(typeof(JumpAttack));
            AddSkill(typeof(JumpRisingAttack));
            AddSkill(typeof(Onrush));
            AddSkill(typeof(SideAttack));
            AddSkill(typeof(Block));
            AddSkill(typeof(BlockCounter));

            AddSkill(typeof(EGOAirBackAttack));
            AddSkill(typeof(EGOAirBackFallingAttack));
            AddSkill(typeof(EGOAirBasicAttack));
            AddSkill(typeof(EGOBackAttack));
            AddSkill(typeof(EGOBasicAttack));
            AddSkill(typeof(EGODodge));
            AddSkill(typeof(EGOForwardAttack));
            AddSkill(typeof(EGOJumpAttack));
            AddSkill(typeof(EGOJumpRisingAttack));
            AddSkill(typeof(EGOSideAttack));
            AddSkill(typeof(EGOBlock));
            AddSkill(typeof(EGOBlockCounter));


            // Arbiter
            AddSkill(typeof(FairyCombo));

            AddSkill(typeof(Lock));
            AddSkill(typeof(Unlock));

            AddSkill(typeof(CastPillar));
            AddSkill(typeof(ChannelPillar));
            AddSkill(typeof(ChargePillarSpear));
            AddSkill(typeof(ThrowPillarSpear));

            AddSkill(typeof(CastShockwave));
            AddSkill(typeof(ChannelShockwave));

            AddSkill(typeof(LockState));
        }

        internal static void AddSkill(Type t)
        {
            entityStates.Add(t);
        }
    }
}