using BepInEx.Configuration;
using UnityEngine;

namespace RiskOfRuinaMod.Modules
{
    public static class Config
    {
        public static ConfigEntry<bool> themeMusic;
        public static ConfigEntry<bool> snapLevel;
        public static ConfigEntry<bool> arbiterSound;

        public static ConfigEntry<float> statRatio;
        public static ConfigEntry<float> moveSpeedMult;
        public static ConfigEntry<float> attackSpeedMult;
        public static ConfigEntry<float> sprintSpeedMult;
        public static ConfigEntry<float> emotionRatio;
        public static ConfigEntry<float> emotionDecay;
        public static ConfigEntry<float> EGOAgeRatio;
        public static ConfigEntry<float> redMistBuffDamage;

        public static ConfigEntry<bool> iframeOverlay;
        public static ConfigEntry<bool> redMistCoatShader;

        public static void ReadConfig()
        {
            themeMusic = RiskOfRuinaPlugin.instance.Config.Bind<bool>(new ConfigDefinition("Audio", "Theme Music"), true, new ConfigDescription("Set to false to disable theme music on Red Mist transformation."));
            snapLevel = RiskOfRuinaPlugin.instance.Config.Bind<bool>(new ConfigDefinition("Audio", "Snap On Level Up"), true, new ConfigDescription("Set to false to disable snapping on level up."));
            arbiterSound = RiskOfRuinaPlugin.instance.Config.Bind<bool>(new ConfigDefinition("Audio", "Arbiter Primary Attack"), true, new ConfigDescription("Set to false to make Arbiter's Primary less grating."));

            statRatio = RiskOfRuinaPlugin.instance.Config.Bind<float>(new ConfigDefinition("Red Mist :: Stats", "Stat Ratio"), 0.0f, new ConfigDescription("Alter this to change how much speed gets converted into damage. 1.0 for none, 0.0 for all."));
            moveSpeedMult = RiskOfRuinaPlugin.instance.Config.Bind<float>(new ConfigDefinition("Red Mist :: Stats", "Move Speed Multiplier"), 6f, new ConfigDescription("Alter this to change how much 1 move speed is worth in damage."));
            attackSpeedMult = RiskOfRuinaPlugin.instance.Config.Bind<float>(new ConfigDefinition("Red Mist :: Stats", "Attack Speed Multiplier"), 50f, new ConfigDescription("Alter this to change how much 1 attack speed is worth in damage."));
            sprintSpeedMult = RiskOfRuinaPlugin.instance.Config.Bind<float>(new ConfigDefinition("Red Mist :: Stats", "Sprint Speed Multiplier"), 0.2f, new ConfigDescription("Alter this to change how much 1 energy drink is worth for sprint speed."));

            emotionRatio = RiskOfRuinaPlugin.instance.Config.Bind<float>(new ConfigDefinition("Red Mist :: EGO", "EGO Per Hit"), 0.4f, new ConfigDescription("Alter this to change how much EGO is gained per hit."));
            emotionDecay = RiskOfRuinaPlugin.instance.Config.Bind<float>(new ConfigDefinition("Red Mist :: EGO", "EGO Decay"), 0.005f, new ConfigDescription("Alter this to change how fast EGO is lost."));
            EGOAgeRatio = RiskOfRuinaPlugin.instance.Config.Bind<float>(new ConfigDefinition("Red Mist :: EGO", "EGO Decay Increase"), 0.0005f, new ConfigDescription("Alter this to change how much EGO Decay increases per tick while in EGO."));
            redMistBuffDamage = RiskOfRuinaPlugin.instance.Config.Bind<float>(new ConfigDefinition("Red Mist :: EGO", "Buff Damage Increase"), 0.01f, new ConfigDescription("Alter this to change how much a stack of the EGO buff increases your damage, value is a percentage of your total damage."));

            iframeOverlay = RiskOfRuinaPlugin.instance.Config.Bind<bool>(new ConfigDefinition("Red Mist :: Misc", "Iframe Overlay"), true, new ConfigDescription("Set to false to disable character overlay on IFrames."));
            redMistCoatShader = RiskOfRuinaPlugin.instance.Config.Bind<bool>(new ConfigDefinition("Red Mist :: Misc", "EGO Shader"), true, new ConfigDescription("Set to false to disable the usage of non-standard shaders for EGO."));
        }

        // this helper automatically makes config entries for disabling survivors
        internal static ConfigEntry<bool> CharacterEnableConfig(string characterName)
        {
            return RiskOfRuinaPlugin.instance.Config.Bind<bool>(new ConfigDefinition(characterName, "Enabled"), true, new ConfigDescription("Set to false to disable this character"));
        }

        internal static ConfigEntry<bool> ItemEnableConfig(string itemName)
        {
            return RiskOfRuinaPlugin.instance.Config.Bind<bool>(new ConfigDefinition(itemName, "Enabled"), true, new ConfigDescription("Set to false to disable this item"));
        }

        internal static ConfigEntry<bool> EnemyEnableConfig(string characterName)
        {
            return RiskOfRuinaPlugin.instance.Config.Bind<bool>(new ConfigDefinition(characterName, "Enabled"), true, new ConfigDescription("Set to false to disable this enemy"));
        }
    }
}