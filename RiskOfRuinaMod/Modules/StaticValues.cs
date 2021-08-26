using System;

namespace RiskOfRuinaMod.Modules
{
    internal static class StaticValues
    {
        // Red Mist
        internal const float originalMoveSpeed = 10.0f;
        internal const float originalAttackSpeed = 1.2f;

        internal const float basicAttackDamageCoefficient = 2f;
        internal const float basicAttackThirdHitDamageCoefficient = 3f;

        internal const float blockCounterDamageCoefficient = 1.5f;

        internal const float sideAttackDamageCoefficient = 1.25f;

        internal const float jumpAttackDamageCoefficient = 1.5f;

        internal const float backAttackDamageCoefficient = 1.75f;

        internal const float forwardAttackDamageCoefficient = 1.25f;
        internal const float forwardAttackComboDamageCoefficient = 1.0f;

        internal const float airBasicAttackDamageCoefficient = 2f;

        internal const float airBackAttackDamageCoefficient = 2f;
        internal const float airBackSlamAttackDamageCoefficient = 2.0f;

        internal const float onrushDamageCoefficient = 4.0f;

        internal const float horizontalDamageCoefficient = 20.0f;

        internal const float utilityPVPDebuff = 0.2f;
        internal const float counterPVPBoost = 5f;

        internal const float egoPVPMonsterMod = 0.75f;
        internal const float egoPVPCharacterMod = 7.5f;


        // Arbiter
        internal const float fairyDebuffCoefficient = 1.0f;

        internal const float fairyDamageCoefficient = 0.75f;

        internal const float lockDamageCoefficient = 3.0f;

        internal const float shockwaveMinDamageCoefficient = 5f;
        internal const float shockwaveMaxDamageCoefficient = 15f;
        internal const float shockwaveScepterDamageCoefficient = 50f;
        internal const float shockwaveMinRadius = 40f;
        internal const float shockwaveMaxRadius = 40f;
        internal const float shockwaveScepterRadius = 60f;
        internal const float shockwaveMinBarrier = 0.3f;
        internal const float shockwaveMaxBarrier = 0.3f;
        internal const float shockwaveScepterBarrier = 0.5f;
        internal const float shockwaveChannelDuration = 0.4f;
        internal const float shockwaveScepterChannelDuration = 1f;

        internal const float feebleArmorDamageAmount = 0.5f;
        internal const float feebleDamageDamageAmount = 0.5f;

        internal const float pillarRadius = 25f;
        internal const float pillarChannelDuration = 1f;
        internal const float pillarDuration = 10f;
        internal const float pillarDamageCoefficient = 5.0f;

        internal const float pillarSpearRadius = 15f;
        internal const float pillarSpearChargeDuration = 2f;
        internal const float pillarSpearMinDamageCoefficient = 3.0f;
        internal const float pillarSpearMaxDamageCoefficient = 6.0f;
    }
}