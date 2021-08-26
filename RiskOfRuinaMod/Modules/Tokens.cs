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
            LanguageAPI.Add(prefix + "PASSIVE_DESCRIPTION", "Red Mist can <style=cIsUtility>jump twice</style>. All <style=cIsDamage>Attack Speed</style> and <style=cIsUtility>Movement Speed</style> bonuses are converted into <style=cIsDamage>Damage</style>.");
            #endregion

            #region Primary
            LanguageAPI.Add(prefix + "PRIMARY_LEVELSLASH_NAME", "Level Slash");
            LanguageAPI.Add(prefix + "PRIMARY_LEVELSLASH_DESCRIPTION", $"Perform attack combos for varying damage. <color=#7a21a3>This move is affected by directional input/jumping.</color>");

            LanguageAPI.Add(prefix + "PRIMARY_UPSTANDINGSLASH_NAME", "Upstanding Slash");
            LanguageAPI.Add(prefix + "PRIMARY_UPSTANDINGSLASH_DESCRIPTION", $"<style=cIsUtility>Slayer</style>. Perform attack combos for varying damage. <color=#7a21a3>This move is affected by directional input/jumping.</color>");
            #endregion

            #region Secondary
            LanguageAPI.Add(prefix + "SECONDARY_ONRUSH_NAME", "Onrush");
            LanguageAPI.Add(prefix + "SECONDARY_ONRUSH_DESCRIPTION", $"Dash to a targeted enemy and deal <style=cIsDamage>{100f * StaticValues.onrushDamageCoefficient}% damage</style>. If you kill the enemy, <style=cIsUtility>you can dash again</style>.");
            #endregion

            #region Utility
            LanguageAPI.Add(prefix + "UTILITY_DODGE_NAME", "Evade");
            LanguageAPI.Add(prefix + "UTILITY_DODGE_DESCRIPTION", $"<style=cIsUtility>Evade</style> all attacks for a split second while repositioning.");

            LanguageAPI.Add(prefix + "UTILITY_BLOCK_NAME", "Counter");
            LanguageAPI.Add(prefix + "UTILITY_BLOCK_DESCRIPTION", $"<style=cIsUtility>Block</style> all attacks for a split second, then counter for <style=cIsDamage>{100f * StaticValues.blockCounterDamageCoefficient}% of damage recieved</style>. Hold to block additional attacks before countering.");
            #endregion

            #region Special
            LanguageAPI.Add(prefix + "SPECIAL_EGO_NAME", "Red Mist: EGO");
            LanguageAPI.Add(prefix + "SPECIAL_EGO_DESCRIPTION", $"<color=red>100% EGO</color>. Activate EGO Mode, replacing special with <style=cIsDamage>Greater Split</style> and <style=cIsUtility>upgrading your other abilities</style>.");

            LanguageAPI.Add(prefix + "SPECIAL_HORIZONTAL_NAME", "Greater Split: Horizontal");
            LanguageAPI.Add(prefix + "SPECIAL_HORIZONTAL_DESCRIPTION", $"Swing your sword in a massive arc, dealing <style=cIsDamage>{100f * StaticValues.horizontalDamageCoefficient}% damage</style>.");

            LanguageAPI.Add("KEYWORD_EGO", $"<color=red><style=cKeywordName>EGO Mode</style></color><style=cSub>Gain <color=red>EGO</color> by <style=cIsDamage>damaging enemies</style>. Drains over time. While in EGO Mode, gain a <style=cIsDamage>{100f * Config.redMistBuffDamage.Value}% damage increase</style> for every enemy killed.");
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

            LanguageAPI.Add("KEYWORD_FAIRY", $"<color=yellow><style=cKeywordName>Fairy</style></color><style=cSub>Enemies afflicted with " + Helpers.fairyPrefix + $"will take <style=cIsDamage>{100f * StaticValues.fairyDebuffCoefficient}% damage</style> whenever they use an ability.");
            #endregion

            #region Primary
            LanguageAPI.Add(prefix + "PRIMARY_FAIRY_NAME", "Fairy");
            LanguageAPI.Add(prefix + "PRIMARY_FAIRY_DESCRIPTION", $"Fire explosive projectiles for <style=cIsDamage>{100f * StaticValues.fairyDamageCoefficient}% damage</style>.");
            #endregion

            #region Secondary
            LanguageAPI.Add(prefix + "SECONDARY_LOCK_NAME", "Lock");
            LanguageAPI.Add(prefix + "SECONDARY_LOCK_DESCRIPTION", $"<style=cIsUtility>Lock</style> a target enemy and deal <style=cIsDamage>{100f * StaticValues.lockDamageCoefficient}% damage</style>.");

            LanguageAPI.Add("KEYWORD_LOCK", $"<color=yellow><style=cKeywordName>Lock</style></color><style=cSub>Enemies afflicted with <style=cIsUtility>Lock</style> are <style=cIsUtility>frozen in time</style> for the duration. Enemies gain resistance to repeated locking.");

            LanguageAPI.Add(prefix + "SECONDARY_UNLOCK_NAME", "Unlock");
            LanguageAPI.Add(prefix + "SECONDARY_UNLOCK_DESCRIPTION", $"Consume all charges. <style=cIsUtility>Unlock</style> a target ally. Duration is increased for each charge used.");

            LanguageAPI.Add("KEYWORD_UNLOCK", $"<color=yellow><style=cKeywordName>Unlock</style></color><style=cSub>Allies with <style=cIsUtility>Unlock</style> gain <style=cIsUtility>increased damage, armor, regen, attack speed, movement speed, and ability cooldown speed</style> for the duration.");
            #endregion

            #region Utility
            LanguageAPI.Add(prefix + "UTILITY_PILLARS_NAME", "Ominous Pillar");
            LanguageAPI.Add(prefix + "UTILITY_PILLARS_DESCRIPTION", $"A pillar emerges for <style=cIsDamage>{100f * StaticValues.pillarDamageCoefficient}% damage</style>. Everything in the area gains double <style=cIsUtility>movement</style> and <style=cIsDamage>attack</style> speed, and enemy projectiles are <style=cIsUtility>destroyed</color>.");

            LanguageAPI.Add(prefix + "UTILITY_PILLARSSPEAR_NAME", "Pillar Spear");
            LanguageAPI.Add(prefix + "UTILITY_PILLARSSPEAR_DESCRIPTION", $"Charge and launch a pillar for <style=cIsDamage>{100f * StaticValues.pillarSpearMinDamageCoefficient}% - {100f * StaticValues.pillarSpearMaxDamageCoefficient}% damage</style>. All enemies hit will activate their " + Helpers.fairyPrefix + "stacks.");
            #endregion

            #region Special
            LanguageAPI.Add(prefix + "SPECIAL_SHOCKWAVE_NAME", "Shockwave");
            LanguageAPI.Add(prefix + "SPECIAL_SHOCKWAVE_DESCRIPTION", $"Consumes all charges. Fire 3 AOE bursts for <style=cIsDamage>{100f * StaticValues.shockwaveMinDamageCoefficient}% - {100f * StaticValues.shockwaveMaxDamageCoefficient}% damage</style>. Enemies hit are made <style=cIsUtility>Feeble</style> and allies gain <style=cIsUtility>Barrier</style>.");

            LanguageAPI.Add(prefix + "SPECIAL_SCEPTERSHOCKWAVE_NAME", "Undegraded Shockwave");
            LanguageAPI.Add(prefix + "SPECIAL_SCEPTERSHOCKWAVE_DESCRIPTION", $"Consumes all charges. Fire an AOE bursts for <style=cIsDamage>{100f * StaticValues.shockwaveScepterDamageCoefficient}% damage</style>. Enemies hit are made <style=cIsUtility>Feeble</style> and allies gain <style=cIsUtility>Barrier</style>.");

            LanguageAPI.Add("KEYWORD_FEEBLE", $"<color=yellow><style=cKeywordName>Feeble</style></color><style=cSub>Enemies afflicted with <style=cIsUtility>Feeble</style> lose half of their <style=cIsDamage>damage</style> and <style=cIsUtility>armor</style> for the duration.");
            #endregion

            #region Achievements
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_NAME", "Arbiter: Mastery");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_DESC", "As An Arbiter, beat the game or obliterate on Monsoon.");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_UNLOCKABLE_NAME", "Arbiter: Mastery");
            #endregion
            #endregion


            #region BlackSilence
            prefix = RiskOfRuinaPlugin.developerPrefix + "_BLACKSILENCE_BODY_";

            desc = "Black Silence" + Environment.NewLine + Environment.NewLine;

            outro = "..and so he left, with the resolve to make a different choice.";
            outroFailure = "..and so he remained, left with the greatest suffering at the end of it all.";

            LanguageAPI.Add(prefix + "NAME", "Black Silence");
            LanguageAPI.Add(prefix + "DESCRIPTION", desc);
            LanguageAPI.Add(prefix + "SUBTITLE", "Spectre of Vengeance ");
            LanguageAPI.Add(prefix + "LORE", "The Black Silence.");
            LanguageAPI.Add(prefix + "OUTRO_FLAVOR", outro);
            LanguageAPI.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            LanguageAPI.Add(prefix + "DEFAULT_SKIN_NAME", "Default");
            LanguageAPI.Add(prefix + "MASTERY_SKIN_NAME", "Mastery: Waltz in White");
            #endregion

            #region Passive
            LanguageAPI.Add(prefix + "PASSIVE_NAME", "Gebura's Prowess");
            LanguageAPI.Add(prefix + "PASSIVE_DESCRIPTION", "Red Mist can <style=cIsUtility>jump twice</style>. All <style=cIsDamage>Attack Speed</style> and <style=cIsUtility>Movement Speed</style> bonuses are converted into <style=cIsDamage>Damage</style>.");
            #endregion

            #region Primary
            LanguageAPI.Add(prefix + "PRIMARY_LEVELSLASH_NAME", "Level Slash");
            LanguageAPI.Add(prefix + "PRIMARY_LEVELSLASH_DESCRIPTION", $"Perform attack combos for varying damage. <color=#7a21a3>This move is affected by directional input/jumping.</color>");

            LanguageAPI.Add(prefix + "PRIMARY_UPSTANDINGSLASH_NAME", "Upstanding Slash");
            LanguageAPI.Add(prefix + "PRIMARY_UPSTANDINGSLASH_DESCRIPTION", $"<style=cIsUtility>Slayer</style>. Perform attack combos for varying damage. <color=#7a21a3>This move is affected by directional input/jumping.</color>");
            #endregion

            #region Secondary
            LanguageAPI.Add(prefix + "SECONDARY_ONRUSH_NAME", "Onrush");
            LanguageAPI.Add(prefix + "SECONDARY_ONRUSH_DESCRIPTION", $"Dash to a targeted enemy and deal <style=cIsDamage>{100f * StaticValues.onrushDamageCoefficient}% damage</style>. If you kill the enemy, <style=cIsUtility>you can dash again</style>.");
            #endregion

            #region Utility
            LanguageAPI.Add(prefix + "UTILITY_DODGE_NAME", "Evade");
            LanguageAPI.Add(prefix + "UTILITY_DODGE_DESCRIPTION", $"<style=cIsUtility>Evade</style> all attacks for a split second while repositioning.");

            LanguageAPI.Add(prefix + "UTILITY_BLOCK_NAME", "Counter");
            LanguageAPI.Add(prefix + "UTILITY_BLOCK_DESCRIPTION", $"<style=cIsUtility>Block</style> all attacks for a split second, then counter for <style=cIsDamage>{100f * StaticValues.blockCounterDamageCoefficient}% of damage recieved</style>. Hold to block additional attacks before countering.");
            #endregion

            #region Special
            LanguageAPI.Add(prefix + "SPECIAL_EGO_NAME", "Red Mist: EGO");
            LanguageAPI.Add(prefix + "SPECIAL_EGO_DESCRIPTION", $"<color=red>100% EGO</color>. Activate EGO Mode, replacing special with <style=cIsDamage>Greater Split</style> and <style=cIsUtility>upgrading your other abilities</style>.");

            LanguageAPI.Add(prefix + "SPECIAL_HORIZONTAL_NAME", "Greater Split: Horizontal");
            LanguageAPI.Add(prefix + "SPECIAL_HORIZONTAL_DESCRIPTION", $"Swing your sword in a massive arc, dealing <style=cIsDamage>{100f * StaticValues.horizontalDamageCoefficient}% damage</style>.");

            LanguageAPI.Add("KEYWORD_EGO", $"<color=red><style=cKeywordName>EGO Mode</style></color><style=cSub>Gain <color=red>EGO</color> by <style=cIsDamage>damaging enemies</style>. Drains over time. While in EGO Mode, gain a <style=cIsDamage>{100f * Config.redMistBuffDamage.Value}% damage increase</style> for every enemy killed.");
            #endregion

            #region Achievements
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_NAME", "Black Silence: Mastery");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_ACHIEVEMENT_DESC", "As Black Silence, beat the game or obliterate on Monsoon.");
            LanguageAPI.Add(prefix + "MASTERYUNLOCKABLE_UNLOCKABLE_NAME", "Black Silence: Mastery");
            #endregion
            #endregion


            #region Items
            #region Trophy
            LanguageAPI.Add("ARBITERTROPHY_NAME", "An Arbiter's Trophy");
            LanguageAPI.Add("ARBITERTROPHY_PICKUP", "Chance to Lock on hit");
            LanguageAPI.Add("ARBITERTROPHY_DESC",
                "Grants <style=cIsUtility>2%</style> <style=cStack>(+2% per item stack)</style> chance to Lock enemies on hit.");
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
                "Grants <style=cIsDamage>1%</style> <style=cStack>(+1% per item stack)</style> base damage for each unique item that you possess.");
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

            #region WeddingRing
            LanguageAPI.Add("RUINAWEDDINGRING_NAME", "Wedding Ring");
            LanguageAPI.Add("RUINAWEDDINGRING_PICKUP", "Deal more damage near allies with the same item");
            LanguageAPI.Add("RUINAWEDDINGRING_DESC",
                "Deal <style=cIsDamage>10%</style> <style=cStack>(+5% per item stack)</style> more damage for each stack of this item that nearby allies have.");
            LanguageAPI.Add("RUINAWEDDINGRING_LORE",
                "");
            #endregion

            #region UdjatMask
            LanguageAPI.Add("RUINAUDJATMASK_NAME", "Udjat Mask");
            LanguageAPI.Add("RUINAUDJATMASK_PICKUP", "Gain armor when hit");
            LanguageAPI.Add("RUINAUDJATMASK_DESC",
                "Gain <style=cIsUtility>5</style> <style=cStack>(+5 per item stack)</style> armor for 5 seconds each time you are hit.");
            LanguageAPI.Add("RUINAUDJATMASK_LORE",
                "");
            #endregion

            #region Reverberation
            LanguageAPI.Add("RUINAREVERBERATION_NAME", "Reverberation");
            LanguageAPI.Add("RUINAREVERBERATION_PICKUP", "Attacks reflect projectiles");
            LanguageAPI.Add("RUINAREVERBERATION_DESC",
                "All of your attacks <style=cIsUtility>reflect</style> projectiles. Reflection range increases with stacks.");
            LanguageAPI.Add("RUINAREVERBERATION_LORE",
                "");
            #endregion
            #endregion


            #region Equipment
            #region BackwardsClock
            LanguageAPI.Add("RUINABACKWARDSCLOCK_NAME", "Backwards Clock");
            LanguageAPI.Add("RUINABACKWARDSCLOCK_PICKUP", "Sacrifice yourself to resurrect all allies");
            LanguageAPI.Add("RUINABACKWARDSCLOCK_DESC",
                "Sacrifice yourself to resurrect all allies.");
            LanguageAPI.Add("RUINABACKWARDSCLOCK_LORE",
                "");
            #endregion
            #endregion
        }
    }
}