using R2API;
using System;

namespace RiskOfRuinaMod.Modules
{
    internal static class Tokens
    {
        internal static void AddTokens()
        {
            #region RedMist
            string prefix = RiskOfRuinaPlugin.developerPrefix + "_REDMIST_BODY_";

            string desc = "Red Mist is an aggresive melee survivor with a versatile primary attack and a powerful but temporary transformation.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Her primary, Level Slash, performs different combos depending on what directional buttons you are pressing. She is stronger while stationary, but her mobile attacks give her a short burst of invulnerability." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Onrush is a mobility tool that excels at dealing with swarms of weak enemies. Try to target the weakest enemies with it first, so that you can continue chaining it while you get kills." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Evade is a simple but powerful move. Any of her basic attacks (and some specials) can be canceled into Evade, allowing you to react to your enemies whenever needed." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > EGO is Red Mist's transformation ability. Fill her EGO bar by doing damage, large groups of enemies will make this quicker. EGO also becomes easier to gain as a run progresses. Upon transformation, every one of her skills will be modified to be more versatile while doing the same damage. EGO drains faster the longer you are transformed, so you will need to be aggressive to maintain your new state." + Environment.NewLine + Environment.NewLine;

            string outro = "..and so she left, walking out of a sea of pain.";
            string outroFailure = "..and so she remained, lost to hatred.";

            LanguageAPI.Add(prefix + "NAME", "Red Mist");
            LanguageAPI.Add(prefix + "DESCRIPTION", desc);
            LanguageAPI.Add(prefix + "SUBTITLE", "The Strongest ");
            LanguageAPI.Add(prefix + "LORE", "The Red Mist.");
            LanguageAPI.Add(prefix + "OUTRO_FLAVOR", outro);
            LanguageAPI.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            LanguageAPI.Add(prefix + "DEFAULT_SKIN_NAME", "Default");
            LanguageAPI.Add(prefix + "MASTERY_SKIN_NAME", "Mastery: Conductor");
            #endregion

            #region Passive
            LanguageAPI.Add(prefix + "PASSIVE_NAME", "Gebura's Prowess");
            LanguageAPI.Add(prefix + "PASSIVE_DESCRIPTION", "All of Red Mist's Attack and Movement Speed bonuses are converted into Damage. She can also jump twice.");
            #endregion

            #region Primary
            LanguageAPI.Add(prefix + "PRIMARY_LEVELSLASH_NAME", "Level Slash");
            LanguageAPI.Add(prefix + "PRIMARY_LEVELSLASH_DESCRIPTION", $"Perform attack combos for varying damage. <color=#7a21a3>This move is affected by directional input/jumping.</color>");

            LanguageAPI.Add(prefix + "PRIMARY_UPSTANDINGSLASH_NAME", "Upstanding Slash");
            LanguageAPI.Add(prefix + "PRIMARY_UPSTANDINGSLASH_DESCRIPTION", $"Perform attack combos for varying damage. Deals up to 3X damage against low health enemies. <color=#7a21a3>This move is affected by directional input/jumping.</color>");
            #endregion

            #region Secondary
            LanguageAPI.Add(prefix + "SECONDARY_ONRUSH_NAME", "Onrush");
            LanguageAPI.Add(prefix + "SECONDARY_ONRUSH_DESCRIPTION", $"Dash to a targeted enemy and deal <style=cIsDamage>{100f * StaticValues.onrushDamageCoefficient}% damage</style>. If this kills the enemy, the stock is immediately refunded.");
            #endregion

            #region Utility
            LanguageAPI.Add(prefix + "UTILITY_DODGE_NAME", "Evade");
            LanguageAPI.Add(prefix + "UTILITY_DODGE_DESCRIPTION", $"Evade all attacks for a split second while repositioning.");

            LanguageAPI.Add(prefix + "UTILITY_BLOCK_NAME", "Counter");
            LanguageAPI.Add(prefix + "UTILITY_BLOCK_DESCRIPTION", $"Block all attacks for a split second, then counter with a spin that deals <style=cIsDamage>{100f * StaticValues.blockCounterDamageCoefficient}% of damage recieved</style>.");
            #endregion

            #region Special
            LanguageAPI.Add(prefix + "SPECIAL_EGO_NAME", "Red Mist: EGO");
            LanguageAPI.Add(prefix + "SPECIAL_EGO_DESCRIPTION", $"Fill your EGO bar in the bottom left of the screen by damaging enemies. Once it is full, this skill will activate EGO Mode, upgrading all of your abilities.");

            LanguageAPI.Add(prefix + "SPECIAL_HORIZONTAL_NAME", "Greater Split: Horizontal");
            LanguageAPI.Add(prefix + "SPECIAL_HORIZONTAL_DESCRIPTION", $"Jump into the air and swing your sword in a massive arc, dealing <style=cIsDamage>{100f * StaticValues.horizontalDamageCoefficient}% damage</style> to all enemies hit.");
            #endregion

            #region Achievements
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_NAME", "Red Mist: Mastery");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_DESC", "As Red Mist, beat the game or obliterate on Monsoon.");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_UNLOCKABLE_NAME", "Red Mist: Mastery");
            #endregion

            #region Dialogue
            LanguageAPI.Add("BROTHER_SEE_REDMIST_1", "Boss music? Foolish.");
            LanguageAPI.Add("BROTHER_KILL_REDMIST_1", "Silence that noise.");
            #endregion
            #endregion


            #region Arbiter
            prefix = RiskOfRuinaPlugin.developerPrefix + "_ARBITER_BODY_";

            desc = "An Arbiter is a mid-range survivor who can manage large groups of enemies with numerous debuffs and area attacks - and can even disable high priority targets if needed.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Use your basic attack to apply the Fairy debuff to enemies. They will take damage for every stack of Fairy on them every time they attack." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Lock can be used to disable a dangerous enemy while you take care of smaller ones. It will become weaker with repeated uses on the same target." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Pillar will make not only you and your allies faster, but also enemies. Use this with Fairy to make them damage themselves faster." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Shockwave covers a massive area, dealing consistently increasing damage and weakening enemies. Use it while near allies to provide cover. Keep in mind that Shockwave requires all five stocks to cast." + Environment.NewLine + Environment.NewLine;

            outro = "..and so she left, the cycle broken.";
            outroFailure = "'Oh Sorrow, you see, finally I have come to respect you, for I know you will never depart.'";

            LanguageAPI.Add(prefix + "NAME", "An Arbiter");
            LanguageAPI.Add(prefix + "DESCRIPTION", desc);
            LanguageAPI.Add(prefix + "SUBTITLE", "Agent of The Head ");
            LanguageAPI.Add(prefix + "LORE", "A singularity-powered assassin for A-Corp. Meeting one is rare, and living to tell the tale is unheard of.");
            LanguageAPI.Add(prefix + "OUTRO_FLAVOR", outro);
            LanguageAPI.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            LanguageAPI.Add(prefix + "DEFAULT_SKIN_NAME", "Default");
            LanguageAPI.Add(prefix + "MASTERY_SKIN_NAME", "Mastery: Core Suppression");
            LanguageAPI.Add(prefix + "SECOND_SKIN_NAME", "Fire");
            LanguageAPI.Add(prefix + "THIRD_SKIN_NAME", "Turquoise");
            #endregion

            #region Passive
            LanguageAPI.Add(prefix + "PASSIVE_NAME", "F Corp Singularity");
            LanguageAPI.Add(prefix + "PASSIVE_DESCRIPTION", "All attacks apply the " + Helpers.fairyPrefix + "debuff.");
            #endregion

            #region Primary
            LanguageAPI.Add(prefix + "PRIMARY_FAIRY_NAME", "Fairy");
            LanguageAPI.Add(prefix + "PRIMARY_FAIRY_DESCRIPTION", $"Fire explosive projectiles dealing <style=cIsDamage>{100f * StaticValues.fairyDamageCoefficient}% damage</style>.");
            #endregion

            #region Secondary
            LanguageAPI.Add(prefix + "SECONDARY_LOCK_NAME", "Lock");
            LanguageAPI.Add(prefix + "SECONDARY_LOCK_DESCRIPTION", $"Lock a target enemy for <style=cIsDamage>{100f * StaticValues.lockDamageCoefficient}% damage</style> and freeze them for 5 seconds.");

            LanguageAPI.Add(prefix + "SECONDARY_UNLOCK_NAME", "Unlock");
            LanguageAPI.Add(prefix + "SECONDARY_UNLOCK_DESCRIPTION", $"Unlock a target ally and consume all charges. They are Unlocked for a duration of 10 seconds per charge used, boosting their speed and power.");
            #endregion

            #region Utility
            LanguageAPI.Add(prefix + "UTILITY_PILLARS_NAME", "Ominous Pillar");
            LanguageAPI.Add(prefix + "UTILITY_PILLARS_DESCRIPTION", $"Summon a Pillar that deals <style=cIsDamage>{100f * StaticValues.pillarDamageCoefficient}% damage</style> where it emerges. EVERYTHING in the area gains double movement and attack speed, and <color=#ba251a>ALL projectiles</color> are destroyed.");

            LanguageAPI.Add(prefix + "UTILITY_PILLARSSPEAR_NAME", "Pillar Spear");
            LanguageAPI.Add(prefix + "UTILITY_PILLARSSPEAR_DESCRIPTION", $"Charge and throw a Pillar that deals <style=cIsDamage>{100f * StaticValues.pillarSpearMinDamageCoefficient}% - {100f * StaticValues.pillarSpearMaxDamageCoefficient}% damage</style> in an area where it hits. Everything in the area takes damage for all of their " + Helpers.fairyPrefix + "stacks, and loses all of them.");
            #endregion

            #region Special
            LanguageAPI.Add(prefix + "SPECIAL_SHOCKWAVE_NAME", "Shockwave");
            LanguageAPI.Add(prefix + "SPECIAL_SHOCKWAVE_DESCRIPTION", $"Charge to release 3 bursts dealing <style=cIsDamage>{100f * StaticValues.shockwaveMinDamageCoefficient}% - {100f * StaticValues.shockwaveMaxDamageCoefficient}% damage</style> to all enemies in the area. Enemies in the area are weakened and allies are granted a barrier.");

            LanguageAPI.Add(prefix + "SPECIAL_SCEPTERSHOCKWAVE_NAME", "Undegraded Shockwave");
            LanguageAPI.Add(prefix + "SPECIAL_SCEPTERSHOCKWAVE_DESCRIPTION", $"Charge to release a massive burst dealing <style=cIsDamage>{100f * StaticValues.shockwaveScepterDamageCoefficient}% damage</style> to all enemies in the area. Enemies in the area are weakened and allies are granted a barrier.");
            #endregion

            #region Achievements
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_NAME", "Arbiter: Mastery");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_DESC", "As An Arbiter, beat the game or obliterate on Monsoon.");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_UNLOCKABLE_NAME", "Arbiter: Mastery");
            #endregion
            #endregion


            #region Items
            #region Trophy
            LanguageAPI.Add("ARBITERTROPHY_NAME", "An Arbiter's Trophy");
            LanguageAPI.Add("ARBITERTROPHY_PICKUP", "Chance to Lock on hit");
            LanguageAPI.Add("ARBITERTROPHY_DESC",
                "Grants <style=cIsUtility>1%</style> <style=cStack>(+1% per item stack)</style> chance to Lock enemies on hit.");
            LanguageAPI.Add("ARBITERTROPHY_LORE",
                "A trophy from a past battle.");
            #endregion

            #region Tea
            LanguageAPI.Add("RUINABLACKTEA_NAME", "Black Tea");
            LanguageAPI.Add("RUINABLACKTEA_PICKUP", "Chance to apply Fairy on hit");
            LanguageAPI.Add("RUINABLACKTEA_DESC",
                "Grants <style=cIsUtility>10%</style> <style=cStack>(+5% per item stack)</style> chance to apply Fairy to enemies on hit.");
            LanguageAPI.Add("RUINABLACKTEA_LORE",
                "The agony in the tea leaves is palpable.");
            #endregion

            #region Prescript
            LanguageAPI.Add("RUINAPRESCRIPT_NAME", "Prescript");
            LanguageAPI.Add("RUINAPRESCRIPT_PICKUP", "Increase Base Damage by number of unique items");
            LanguageAPI.Add("RUINAPRESCRIPT_DESC",
                "Grants <style=cIsDamage>3%</style> <style=cStack>(+3% per item stack)</style> base damage for each unique item that you possess.");
            LanguageAPI.Add("RUINAPRESCRIPT_LORE",
                "To ●●●. Pet quadrupedal animals five times.");
            #endregion

            #region LiuBadge
            LanguageAPI.Add("RUINALIUBADGE_NAME", "Liu Badge");
            LanguageAPI.Add("RUINALIUBADGE_PICKUP", "Increase Base Damage by number of stages cleared");
            LanguageAPI.Add("RUINALIUBADGE_DESC",
                "Grants <style=cIsDamage>10%</style> <style=cStack>(+5% per item stack)</style> base damage for each stage you have cleared.");
            LanguageAPI.Add("RUINALIUBADGE_LORE",
                "");
            #endregion

            #region WorkshopAmmo
            LanguageAPI.Add("RUINAWORKSHOPAMMO_NAME", "Workshop Ammunition");
            LanguageAPI.Add("RUINAWORKSHOPAMMO_PICKUP", "Deal more damage the further away you are");
            LanguageAPI.Add("RUINAWORKSHOPAMMO_DESC",
                "Deal up to <style=cIsDamage>25%</style> <style=cStack>(+10% per item stack)</style> damage based on how far you are from your target. (Minimum 10 meters distance)");
            LanguageAPI.Add("RUINAWORKSHOPAMMO_LORE",
                "");
            #endregion

            #region MoonlightStone
            LanguageAPI.Add("RUINAMOONLIGHTSTONE_NAME", "Moonlight Stone");
            LanguageAPI.Add("RUINAMOONLIGHTSTONE_PICKUP", "Remove debuffs over time");
            LanguageAPI.Add("RUINAMOONLIGHTSTONE_DESC",
                "Remove <style=cIsUtility>1 stack</style> <style=cStack>(+1 per item stack)</style> of a debuff from yourself every two seconds.");
            LanguageAPI.Add("RUINAMOONLIGHTSTONE_LORE",
                "");
            #endregion
            #endregion
        }
    }
}