using BepInEx.Configuration;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using On.RoR2.UI;
using RoR2.UI;
using UnityEngine.UI;
using RiskOfRuinaMod.Modules.Components;

namespace RiskOfRuinaMod.Modules.Survivors
{
    internal class RedMist : SurvivorBase
    {
        public static GameObject redMistPrefab;

        internal override string bodyName { get; set; } = "RedMist";

        internal override GameObject bodyPrefab { get; set; }
        internal override GameObject displayPrefab { get; set; }

        internal override float sortPosition { get; set; } = 100f;

        internal override ConfigEntry<bool> characterEnabled { get; set; }

        // Skills
        public static SkillDef NormalDodge;
        public static SkillDef EGODodge;

        public static SkillDef NormalBlock;
        public static SkillDef EGOBlock;

        public static SkillDef EGOActivate;
        public static SkillDef HorizontalSlash;

        public static SkillDef BasicAttack;

        internal override BodyInfo bodyInfo { get; set; } = new BodyInfo
        {
            armor = 20f,
            armorGrowth = 0f,
            bodyName = "RedMistBody",
            bodyNameToken = RiskOfRuinaPlugin.developerPrefix + "_REDMIST_BODY_NAME",
            bodyColor = Color.red,
            characterPortrait = Modules.Assets.LoadCharacterIcon("RedMist"),
            crosshair = Modules.Assets.LoadCrosshair("SimpleDot"),
            damage = 12f,
            healthGrowth = 33f,
            healthRegen = 1f,
            jumpCount = 1,
            maxHealth = 110f,
            subtitleNameToken = RiskOfRuinaPlugin.developerPrefix + "_REDMIST_BODY_SUBTITLE",
            podPrefab = Resources.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod"),
            moveSpeed = StaticValues.originalMoveSpeed,
            attackSpeed = StaticValues.originalAttackSpeed,
            sprintSpeedMult = 1.5f
        };

        internal static Material redMistMat = Modules.Assets.CreateMaterial("matRedMist", 1f, Color.red, 1.0f);
        internal static Material mimicryMat = Modules.Assets.CreateMaterial("matMimicry", 0f, Color.white, 1.0f);
        internal static Material coatMat = Modules.Assets.CreateMaterial("matCoat", 0f, Color.white, 1.0f);
        internal static Material coatEGOMat = Assets.mainAssetBundle.LoadAsset<Material>("matCoatEGO");
        internal override int mainRendererIndex { get; set; } = 3;

        internal override CustomRendererInfo[] customRendererInfos { get; set; } = new CustomRendererInfo[] {
                new CustomRendererInfo
                {
                    childName = "CoatNormal",
                    material = coatMat
                },
                new CustomRendererInfo
                {
                    childName = "CoatEGO",
                    material = coatEGOMat
                },
                new CustomRendererInfo
                {
                    childName = "Mimicry",
                    material = mimicryMat
                },
                new CustomRendererInfo
                {
                    childName = "Model",
                    material = redMistMat
                }};

        internal override Type characterMainState { get; set; } = typeof(EntityStates.GenericCharacterMain);

        // item display stuffs
        internal override ItemDisplayRuleSet itemDisplayRuleSet { get; set; }
        internal override List<ItemDisplayRuleSet.KeyAssetRuleGroup> itemDisplayRules { get; set; }

        internal override UnlockableDef characterUnlockableDef { get; set; }
        private static UnlockableDef masterySkinUnlockableDef;

        internal override void InitializeCharacter()
        {
            if (!Config.redMistCoatShader.Value)
            {
                coatEGOMat = Modules.Assets.CreateMaterial("matCoatEGO", 0f, Color.red, 1.0f);

                customRendererInfos = new CustomRendererInfo[] {
                new CustomRendererInfo
                {
                    childName = "CoatNormal",
                    material = coatMat
                },
                new CustomRendererInfo
                {
                    childName = "CoatEGO",
                    material = coatEGOMat
                },
                new CustomRendererInfo
                {
                    childName = "Mimicry",
                    material = mimicryMat
                },
                new CustomRendererInfo
                {
                    childName = "Model",
                    material = redMistMat
                }};
            }

            base.InitializeCharacter();

            redMistPrefab = bodyPrefab;

            this.bodyPrefab.AddComponent<Modules.Components.TargetTracker>();
            this.bodyPrefab.AddComponent<Modules.Components.RedMistEmotionComponent>();
            this.bodyPrefab.AddComponent<Modules.Components.RedMistStatTracker>();
            On.RoR2.UI.HUD.Awake += new On.RoR2.UI.HUD.hook_Awake(RedMist.HUDAwake);
        }

        internal override void InitializeUnlockables()
        {
            masterySkinUnlockableDef = Modules.Unlockables.AddUnlockable<Achievements.RedMistMasteryAchievement>(true);
        }

        internal override void InitializeDoppelganger()
        {
            Modules.Prefabs.CreateGenericDoppelganger(instance.bodyPrefab, bodyName + "MonsterMaster", "Loader");
        }

        internal override void InitializeHitboxes()
        {
            ChildLocator childLocator = bodyPrefab.GetComponentInChildren<ChildLocator>();
            GameObject model = childLocator.gameObject;

            Transform hitboxTransform = childLocator.FindChild("BasicHitbox");
            Modules.Prefabs.SetupHitbox(model, hitboxTransform, "Basic");
            hitboxTransform = childLocator.FindChild("BasicThirdHitbox");
            Modules.Prefabs.SetupHitbox(model, hitboxTransform, "BasicThird");
            hitboxTransform = childLocator.FindChild("ForwardHitbox");
            Modules.Prefabs.SetupHitbox(model, hitboxTransform, "Forward");
            hitboxTransform = childLocator.FindChild("BackHitbox");
            Modules.Prefabs.SetupHitbox(model, hitboxTransform, "Back");
            hitboxTransform = childLocator.FindChild("JumpHitbox");
            Modules.Prefabs.SetupHitbox(model, hitboxTransform, "Jump");
            hitboxTransform = childLocator.FindChild("SideHitbox");
            Modules.Prefabs.SetupHitbox(model, hitboxTransform, "Side");
            hitboxTransform = childLocator.FindChild("AirBasicHitbox");
            Modules.Prefabs.SetupHitbox(model, hitboxTransform, "AirBasic");
            hitboxTransform = childLocator.FindChild("BlockCounterHitbox");
            Modules.Prefabs.SetupHitbox(model, hitboxTransform, "BlockCounter");

            hitboxTransform = childLocator.FindChild("EGOBasicHitbox");
            Modules.Prefabs.SetupHitbox(model, hitboxTransform, "EGOBasic");
            hitboxTransform = childLocator.FindChild("EGOBasicThirdHitbox");
            Modules.Prefabs.SetupHitbox(model, hitboxTransform, "EGOBasicThird");
            hitboxTransform = childLocator.FindChild("EGOForwardHitbox");
            Modules.Prefabs.SetupHitbox(model, hitboxTransform, "EGOForward");
            hitboxTransform = childLocator.FindChild("EGOBackHitbox");
            Modules.Prefabs.SetupHitbox(model, hitboxTransform, "EGOBack");
            hitboxTransform = childLocator.FindChild("EGOSideHitbox");
            Modules.Prefabs.SetupHitbox(model, hitboxTransform, "EGOSide");
            hitboxTransform = childLocator.FindChild("EGOJumpHitbox");
            Modules.Prefabs.SetupHitbox(model, hitboxTransform, "EGOJump");
            hitboxTransform = childLocator.FindChild("EGOAirBasicHitbox");
            Modules.Prefabs.SetupHitbox(model, hitboxTransform, "EGOAirBasic");
            hitboxTransform = childLocator.FindChild("HorizontalHitbox");
            Modules.Prefabs.SetupHitbox(model, hitboxTransform, "Horizontal");
        }

        internal override void InitializeSkills()
        {
            Modules.Skills.CreateSkillFamilies(bodyPrefab);

            string prefix = RiskOfRuinaPlugin.developerPrefix;

            InitializeSkills(bodyPrefab, prefix);
        }

        public static void InitializeSkills(GameObject bodyPrefab, string prefix)
        {
            SkillLocator skillLocator = bodyPrefab.GetComponent<SkillLocator>();

            #region Passive
            skillLocator.passiveSkill.enabled = true;
            skillLocator.passiveSkill.skillNameToken = prefix + "_REDMIST_BODY_PASSIVE_NAME";
            skillLocator.passiveSkill.skillDescriptionToken = prefix + "_REDMIST_BODY_PASSIVE_DESCRIPTION";
            skillLocator.passiveSkill.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texRedMistPassiveIcon");
            #endregion

            #region Primary
            BasicAttack = Modules.Skills.CreatePrimarySkillDef(new EntityStates.SerializableEntityStateType(typeof(SkillStates.BasicStringStart)), "Body", prefix + "_REDMIST_BODY_PRIMARY_LEVELSLASH_NAME", prefix + "_REDMIST_BODY_PRIMARY_LEVELSLASH_DESCRIPTION", Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texRedMistPrimaryIcon"), false);
            Modules.Skills.AddPrimarySkill(bodyPrefab, BasicAttack);
            #endregion

            #region Secondary
            SkillDef onrushSkillDef = Modules.Skills.CreateTrackerSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_REDMIST_BODY_SECONDARY_ONRUSH_NAME",
                skillNameToken = prefix + "_REDMIST_BODY_SECONDARY_ONRUSH_NAME",
                skillDescriptionToken = prefix + "_REDMIST_BODY_SECONDARY_ONRUSH_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texRedMistSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Onrush)),
                activationStateMachineName = "Body",
                baseMaxStock = 1,
                baseRechargeInterval = 8f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1
            });

            Modules.Skills.AddSecondarySkills(bodyPrefab, onrushSkillDef);
            #endregion

            #region Utility
            NormalDodge = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_REDMIST_BODY_UTILITY_DODGE_NAME",
                skillNameToken = prefix + "_REDMIST_BODY_UTILITY_DODGE_NAME",
                skillDescriptionToken = prefix + "_REDMIST_BODY_UTILITY_DODGE_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texRedMistUtilityIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Dodge)),
                activationStateMachineName = "Body",
                baseMaxStock = 1,
                baseRechargeInterval = 0,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 0,
                stockToConsume = 0,
            });

            NormalBlock = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_REDMIST_BODY_UTILITY_BLOCK_NAME",
                skillNameToken = prefix + "_REDMIST_BODY_UTILITY_BLOCK_NAME",
                skillDescriptionToken = prefix + "_REDMIST_BODY_UTILITY_BLOCK_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texRedMistUtilityTwoIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Block)),
                activationStateMachineName = "Body",
                baseMaxStock = 1,
                baseRechargeInterval = 0,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 0,
                stockToConsume = 0,
            });

            Modules.Skills.AddUtilitySkills(bodyPrefab, NormalDodge, NormalBlock);

            EGODodge = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_REDMIST_BODY_UTILITY_DODGE_NAME",
                skillNameToken = prefix + "_REDMIST_BODY_UTILITY_DODGE_NAME",
                skillDescriptionToken = prefix + "_REDMIST_BODY_UTILITY_DODGE_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texRedMistUtilityIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.EGODodge)),
                activationStateMachineName = "Slide",
                baseMaxStock = 1,
                baseRechargeInterval = 0,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 0,
                stockToConsume = 0,
            });

            Skills.skillDefs.Add(EGODodge);

            EGOBlock = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_REDMIST_BODY_UTILITY_BLOCK_NAME",
                skillNameToken = prefix + "_REDMIST_BODY_UTILITY_BLOCK_NAME",
                skillDescriptionToken = prefix + "_REDMIST_BODY_UTILITY_BLOCK_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texRedMistUtilityTwoIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.EGOBlock)),
                activationStateMachineName = "Slide",
                baseMaxStock = 1,
                baseRechargeInterval = 0,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 0,
                stockToConsume = 0,
            });


            Skills.skillDefs.Add(NormalBlock);
            #endregion

            #region Special
            EGOActivate = Modules.Skills.CreateEGOSkillDef(new SkillDefInfo
            {
                skillName = prefix + "REDMIST_BODY_SPECIAL_EGO_NAME",
                skillNameToken = prefix + "_REDMIST_BODY_SPECIAL_EGO_NAME",
                skillDescriptionToken = prefix + "_REDMIST_BODY_SPECIAL_EGO_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texRedMistSpecialIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.EGOActivate)),
                activationStateMachineName = "Body",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = true,
                cancelSprintingOnActivation = true,
                rechargeStock = 100,
                requiredStock = 0,
                stockToConsume = 0,
                keywordTokens = new string[]
                {
                    "KEYWORD_EGO"
                }
            });

            Modules.Skills.AddSpecialSkills(bodyPrefab, EGOActivate);

            HorizontalSlash = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "REDMIST_BODY_SPECIAL_HORIZONTAL_NAME",
                skillNameToken = prefix + "_REDMIST_BODY_SPECIAL_HORIZONTAL_NAME",
                skillDescriptionToken = prefix + "_REDMIST_BODY_SPECIAL_HORIZONTAL_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texRedMistSpecialTwoIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.EGOHorizontal)),
                activationStateMachineName = "Body",
                baseMaxStock = 1,
                baseRechargeInterval = 20f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1
            });

            Skills.skillDefs.Add(HorizontalSlash);
            #endregion
        }

        internal override void InitializeSkins()
        {
            GameObject model = bodyPrefab.GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
            CharacterModel characterModel = model.GetComponent<CharacterModel>();

            ModelSkinController skinController = model.AddComponent<ModelSkinController>();
            ChildLocator childLocator = model.GetComponent<ChildLocator>();

            SkinnedMeshRenderer mainRenderer = characterModel.mainSkinnedMeshRenderer;

            CharacterModel.RendererInfo[] defaultRenderers = characterModel.baseRendererInfos;

            List<SkinDef> skins = new List<SkinDef>();

            #region DefaultSkin
            SkinDef defaultSkin = Modules.Skins.CreateSkinDef(RiskOfRuinaPlugin.developerPrefix + "_REDMIST_BODY_DEFAULT_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texRedMistMainSkin"),
                defaultRenderers,
                mainRenderer,
                model);

            defaultSkin.meshReplacements = new SkinDef.MeshReplacement[]
            {
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshRedMistCoat"),
                    renderer = defaultRenderers[0].renderer
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshRedMistCoat"),
                    renderer = defaultRenderers[1].renderer
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshMimicry"),
                    renderer = defaultRenderers[2].renderer
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshRedMist"),
                    renderer = defaultRenderers[3].renderer
                }
            };

            skins.Add(defaultSkin);
            #endregion

            #region ArgaliaSkin
            CharacterModel.RendererInfo[] secondRendererInfos = new CharacterModel.RendererInfo[defaultRenderers.Length];
            defaultRenderers.CopyTo(secondRendererInfos, 0);

            secondRendererInfos[0].defaultMaterial = Assets.CreateMaterial("matArgaliaCoat", 0f, Color.white, 1.0f);
            secondRendererInfos[1].defaultMaterial = Assets.CreateMaterial("matArgaliaCoat", 0f, Color.white, 1.0f);
            secondRendererInfos[2].defaultMaterial = Assets.CreateMaterial("matVibrato", 0.9f, new Color(0, 0.95f, 1), 1.0f);
            secondRendererInfos[3].defaultMaterial = Assets.CreateMaterial("matArgalia", 0.9f, new Color(0, 0.95f, 1), 1.0f);

            SkinDef secondSkin = Modules.Skins.CreateSkinDef(RiskOfRuinaPlugin.developerPrefix + "_REDMIST_BODY_MASTERY_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texRedMistSecondSkin"),
                secondRendererInfos,
                mainRenderer,
                model,
                masterySkinUnlockableDef);

            secondSkin.meshReplacements = new SkinDef.MeshReplacement[]
            {
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshArgaliaCoat"),
                    renderer = defaultRenderers[0].renderer
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshArgaliaCoat"),
                    renderer = defaultRenderers[1].renderer
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshVibrato"),
                    renderer = defaultRenderers[2].renderer
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshArgalia"),
                    renderer = defaultRenderers[3].renderer
                }
            };

            skins.Add(secondSkin);
            #endregion

            skinController.skins = skins.ToArray();
        }

        internal static void HUDAwake(On.RoR2.UI.HUD.orig_Awake orig, RoR2.UI.HUD self)
        {
            orig.Invoke(self);
            EmotionHUD emotionHUD = self.gameObject.AddComponent<EmotionHUD>();
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Assets.mainAssetBundle.LoadAsset<GameObject>("EmotionGauge"), self.transform.Find("MainContainer").Find("MainUIArea").Find("SpringCanvas").Find("BottomLeftCluster"));
            //GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Assets.mainAssetBundle.LoadAsset<GameObject>("EmotionGauge"), self.mainContainer.transform);
            gameObject.GetComponent<RectTransform>().localPosition = Vector3.zero;
            gameObject.GetComponent<RectTransform>().localScale = Vector3.one;
            gameObject.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 0.0f);
            gameObject.GetComponent<RectTransform>().anchorMax = new Vector2(1f, 0.2f);
            gameObject.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -55f);
            //gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(-5.5f, -170f);
            //gameObject.GetComponent<RectTransform>().localScale = new Vector3(0.74f, 0.58f, 1f);
            emotionHUD.emotionGauge = gameObject;
            emotionHUD.emotionFill = gameObject.transform.Find("GaugeFill").gameObject.GetComponent<Image>();
        }

        internal override void SetItemDisplays()
        {
            itemDisplayRules = new List<ItemDisplayRuleSet.KeyAssetRuleGroup>();

            // add item displays here
            //  HIGHLY recommend using KingEnderBrine's ItemDisplayPlacementHelper mod for this
            #region Item Displays
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Jetpack,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBugWings"),
childName = "Chest",
localPos = new Vector3(-0.00399F, 0.08923F, -0.18541F),
localAngles = new Vector3(351.3922F, 359.5073F, 358.4317F),
localScale = new Vector3(0.2F, 0.2F, 0.2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.GoldGat,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayGoldGat"),
childName = "Head",
localPos = new Vector3(0F, 0F, 0F),
localAngles = new Vector3(2.56457F, 84.09991F, 223.4565F),
localScale = new Vector3(0.2F, 0.2F, 0.2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.BFG,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBFG"),
childName = "Chest",
localPos = new Vector3(0.0782F, 0.33674F, -0.00285F),
localAngles = new Vector3(0F, 0F, 313.6211F),
localScale = new Vector3(0.3F, 0.3F, 0.3F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.CritGlasses,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayGlasses"),
childName = "Head",
localPos = new Vector3(0F, 0.18391F, -0.01779F),
localAngles = new Vector3(270.6798F, -0.00341F, 0.00345F),
localScale = new Vector3(0.16666F, 0.15727F, 0.15727F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Syringe,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplaySyringeCluster"),
childName = "UpperArmR",
localPos = new Vector3(0.01997F, -0.02756F, 0.00693F),
localAngles = new Vector3(309.6017F, 26.74006F, 192.8567F),
localScale = new Vector3(0.1F, 0.1F, 0.1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Behemoth,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBehemoth"),
childName = "Chest",
localPos = new Vector3(0.15914F, 0.28954F, -0.2313F),
localAngles = new Vector3(342.1061F, 180F, 0F),
localScale = new Vector3(0.06017F, 0.06017F, 0.06017F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Missile,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayMissileLauncher"),
childName = "Chest",
localPos = new Vector3(-0.26159F, 0.37173F, -0.13857F),
localAngles = new Vector3(0F, 0F, 51.9225F),
localScale = new Vector3(0.0431F, 0.0431F, 0.0431F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Dagger,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayDagger"),
childName = "Chest",
localPos = new Vector3(-0.0553F, 0.2856F, 0.0945F),
localAngles = new Vector3(334.8839F, 31.5284F, 34.6784F),
localScale = new Vector3(1.2428F, 1.2428F, 1.2299F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Hoof,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayHoof"),
childName = "CalfR",
localPos = new Vector3(-0.00995F, 0.32262F, -0.08719F),
localAngles = new Vector3(70.62775F, 0.70107F, 1.03991F),
localScale = new Vector3(0.09833F, 0.08715F, 0.0758F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ChainLightning,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayUkulele"),
childName = "Chest",
localPos = new Vector3(-0.0011F, -0.0095F, -0.19995F),
localAngles = new Vector3(0F, 180F, 89.3997F),
localScale = new Vector3(0.4749F, 0.4749F, 0.4749F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.GhostOnKill,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayMask"),
childName = "Head",
localPos = new Vector3(0F, 0.1343F, -0.00508F),
localAngles = new Vector3(281.8823F, 180.0002F, 179.9998F),
localScale = new Vector3(0.54317F, 0.54317F, 0.54317F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Mushroom,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayMushroom"),
childName = "Chest",
localPos = new Vector3(-0.0139F, 0.21583F, -0.16674F),
localAngles = new Vector3(0.88879F, 277.6868F, 90.00072F),
localScale = new Vector3(0.0501F, 0.0501F, 0.0501F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.AttackSpeedOnCrit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayWolfPelt"),
childName = "Head",
localPos = new Vector3(0F, 0.11155F, -0.0958F),
localAngles = new Vector3(285.2229F, 164.0233F, 195.6664F),
localScale = new Vector3(0.29076F, 0.29076F, 0.29076F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.BleedOnHit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayTriTip"),
childName = "CalfL",
localPos = new Vector3(-0.0533F, 0.24218F, -0.01261F),
localAngles = new Vector3(277.2209F, 152.5847F, 150.3102F),
localScale = new Vector3(0.2615F, 0.2615F, 0.2615F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.WardOnLevel,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayWarbanner"),
childName = "Pelvis",
localPos = new Vector3(0F, 0.0817F, -0.0955F),
localAngles = new Vector3(0F, 0F, 90F),
localScale = new Vector3(0.01F, 0.01F, 0.01F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.HealOnCrit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayScythe"),
childName = "Chest",
localPos = new Vector3(0.08762F, 0.44194F, -0.14627F),
localAngles = new Vector3(308.0577F, 353.5334F, 106.1726F),
localScale = new Vector3(0.07529F, 0.07529F, 0.07529F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.HealWhileSafe,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplaySnail"),
childName = "Chest",
localPos = new Vector3(0.12272F, 0.2749F, -0.13173F),
localAngles = new Vector3(289.7526F, 338.3101F, 358.7119F),
localScale = new Vector3(0.0289F, 0.0289F, 0.0289F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Clover,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayClover"),
childName = "Gun",
localPos = new Vector3(0.0004F, 0.1094F, -0.1329F),
localAngles = new Vector3(85.6192F, 0.0001F, 179.4897F),
localScale = new Vector3(0.2749F, 0.2749F, 0.2749F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.BarrierOnOverHeal,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayAegis"),
childName = "LowerArmL",
localPos = new Vector3(-0.05684F, 0.05248F, -0.0138F),
localAngles = new Vector3(87.2154F, 101.9521F, 357.0091F),
localScale = new Vector3(0.2849F, 0.2849F, 0.2849F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.GoldOnHit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBoneCrown"),
childName = "Head",
localPos = new Vector3(0F, 0.04786F, -0.0534F),
localAngles = new Vector3(276.2473F, 180.0004F, 179.9996F),
localScale = new Vector3(0.76369F, 0.76369F, 0.76369F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.WarCryOnMultiKill,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayPauldron"),
childName = "UpperArmR",
localPos = new Vector3(0.05202F, 0.08054F, 0.01918F),
localAngles = new Vector3(74.69036F, 80.85159F, 19.4037F),
localScale = new Vector3(0.45128F, 0.45128F, 0.45128F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.SprintArmor,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBuckler"),
childName = "LowerArmR",
localPos = new Vector3(0.00301F, 0.17958F, 0.01516F),
localAngles = new Vector3(1.19919F, 54.70515F, 126.3237F),
localScale = new Vector3(0.17511F, 0.17511F, 0.17511F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.IceRing,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayIceRing"),
childName = "HandR",
localPos = new Vector3(-0.0068F, 0.08889F, 0.02841F),
localAngles = new Vector3(274.3965F, 90F, 270F),
localScale = new Vector3(0.1F, 0.1F, 0.1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.FireRing,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayFireRing"),
childName = "HandR",
localPos = new Vector3(0.007F, 0.09038F, 0.01348F),
localAngles = new Vector3(276.5588F, 355.0313F, 5.83008F),
localScale = new Vector3(0.1F, 0.1F, 0.1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.UtilitySkillMagazine,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayAfterburnerShoulderRing"),
childName = "Chest",
localPos = new Vector3(0F, 0.15024F, 0.00316F),
localAngles = new Vector3(355.8665F, 180F, 180F),
localScale = new Vector3(0.38391F, 0.38391F, 0.38391F),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayAfterburnerShoulderRing"),
childName = "Chest",
localPos = new Vector3(0F, 0F, 0F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.01F, 0.01F, 0.01F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.JumpBoost,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayWaxBird"),
childName = "UpperArmR",
localPos = new Vector3(-0.08867F, 0.25236F, -0.05454F),
localAngles = new Vector3(3.35221F, 131.5712F, 163.7028F),
localScale = new Vector3(0.61867F, 0.5253F, 0.5253F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ArmorReductionOnHit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayWarhammer"),
childName = "Head",
localPos = new Vector3(0.09652F, 0.08089F, 0.0587F),
localAngles = new Vector3(341.414F, 10.86936F, 95.12854F),
localScale = new Vector3(0.02584F, 0.02584F, 0.02584F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.NearbyDamageBonus,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayDiamond"),
childName = "Sword",
localPos = new Vector3(-0.002F, 0.1828F, 0F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.1236F, 0.1236F, 0.1236F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ArmorPlate,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayRepulsionArmorPlate"),
childName = "UpperArmL",
localPos = new Vector3(-0.08627F, 0.00747F, -0.02916F),
localAngles = new Vector3(302.5928F, 295.7042F, 356.4166F),
localScale = new Vector3(0.1971F, 0.1971F, 0.1971F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.CommandMissile,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayMissileRack"),
childName = "Head",
localPos = new Vector3(0F, -0.04434F, -0.0492F),
localAngles = new Vector3(43.909F, 0.00004F, 180F),
localScale = new Vector3(0.26871F, 0.26871F, 0.26871F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Feather,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayFeather"),
childName = "Chest",
localPos = new Vector3(-0.05033F, 0.21104F, -0.09953F),
localAngles = new Vector3(322.391F, 0F, 0F),
localScale = new Vector3(0.0285F, 0.0285F, 0.0285F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Crowbar,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayCrowbar"),
childName = "Head",
localPos = new Vector3(0F, 0.03227F, -0.0389F),
localAngles = new Vector3(0F, 0F, 180F),
localScale = new Vector3(0.13052F, 0.13052F, 0.13052F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.FallBoots,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayGravBoots"),
childName = "CalfL",
localPos = new Vector3(-0.0038F, 0.3729F, -0.0046F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.1485F, 0.1485F, 0.1485F),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayGravBoots"),
childName = "CalfR",
localPos = new Vector3(-0.0038F, 0.3729F, -0.0046F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.1485F, 0.1485F, 0.1485F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ExecuteLowHealthElite,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayGuillotine"),
childName = "Pelvis",
localPos = new Vector3(-0.14251F, -0.01864F, 0.00889F),
localAngles = new Vector3(280.392F, 55.00724F, 37.32348F),
localScale = new Vector3(0.16115F, 0.16115F, 0.16115F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.EquipmentMagazine,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBattery"),
childName = "Chest",
localPos = new Vector3(0F, 0F, 0F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.01F, 0.01F, 0.01F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.NovaOnHeal,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayDevilHorns"),
childName = "Head",
localPos = new Vector3(0.02703F, 0.06073F, -0.02988F),
localAngles = new Vector3(13.41394F, 66.95353F, 8.06389F),
localScale = new Vector3(0.5349F, 0.5349F, 0.5349F),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayDevilHorns"),
childName = "Head",
localPos = new Vector3(-0.02703F, 0.06073F, -0.02988F),
localAngles = new Vector3(9.55873F, 105.6705F, 11.71106F),
localScale = new Vector3(0.5349F, 0.5349F, -0.5349F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Infusion,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayInfusion"),
childName = "Base",
localPos = new Vector3(0.08521F, 0.19861F, 0.06679F),
localAngles = new Vector3(0F, 50.33201F, 0F),
localScale = new Vector3(0.31127F, 0.31127F, 0.31127F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Medkit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayMedkit"),
childName = "Chest",
localPos = new Vector3(0.0039F, 0.13044F, 0.04305F),
localAngles = new Vector3(290F, 180F, 0F),
localScale = new Vector3(0.01585F, 0.01585F, 0.01585F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Bandolier,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBandolier"),
childName = "Chest",
localPos = new Vector3(0.01665F, -0.0014F, 0.02336F),
localAngles = new Vector3(283.7496F, 269.9772F, 89.84225F),
localScale = new Vector3(0.24379F, 0.35034F, 0.35034F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.BounceNearby,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayHook"),
childName = "Chest",
localPos = new Vector3(-0.17947F, 0.18036F, -0.00389F),
localAngles = new Vector3(290.3197F, 88.99999F, 0F),
localScale = new Vector3(0.214F, 0.214F, 0.214F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.IgniteOnKill,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayGasoline"),
childName = "Chest",
localPos = new Vector3(-0.1039F, 0.32706F, -0.20256F),
localAngles = new Vector3(78.9426F, 193.0253F, 177.007F),
localScale = new Vector3(0.3165F, 0.3165F, 0.3165F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.StunChanceOnHit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayStunGrenade"),
childName = "Base",
localPos = new Vector3(0.11151F, 0.23955F, 0.00624F),
localAngles = new Vector3(1.79873F, 180F, 180F),
localScale = new Vector3(0.41355F, 0.41355F, 0.41355F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Firework,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayFirework"),
childName = "HandR",
localPos = new Vector3(0.0086F, 0.0069F, 0.0565F),
localAngles = new Vector3(283.2718F, 0.33557F, 74.63754F),
localScale = new Vector3(0.1194F, 0.1194F, 0.1194F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.LunarDagger,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayLunarDagger"),
childName = "Chest",
localPos = new Vector3(-0.09689F, 0.4172F, -0.14314F),
localAngles = new Vector3(350.8387F, 348.7021F, 268.0342F),
localScale = new Vector3(0.11234F, 0.11234F, 0.11234F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Knurl,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayKnurl"),
childName = "Chest",
localPos = new Vector3(-0.00777F, 0.11018F, 0.01039F),
localAngles = new Vector3(78.87074F, 36.6722F, 105.8275F),
localScale = new Vector3(0.03847F, 0.03847F, 0.03847F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.BeetleGland,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBeetleGland"),
childName = "Chest",
localPos = new Vector3(0.10475F, -0.00489F, -0.07403F),
localAngles = new Vector3(359.9584F, 0.1329F, 39.8304F),
localScale = new Vector3(0.0553F, 0.0553F, 0.0553F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.SprintBonus,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplaySoda"),
childName = "Head",
localPos = new Vector3(0F, 0.21324F, 0.06803F),
localAngles = new Vector3(71.27757F, 180.0012F, 71.01811F),
localScale = new Vector3(0.1655F, 0.1655F, 0.1655F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.SecondarySkillMagazine,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayDoubleMag"),
childName = "Chest",
localPos = new Vector3(-0.13208F, 0.12727F, 0.10528F),
localAngles = new Vector3(340.1868F, 55.4414F, 169.2369F),
localScale = new Vector3(0.0441F, 0.0441F, 0.0441F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.StickyBomb,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayStickyBomb"),
childName = "Pelvis",
localPos = new Vector3(0.0594F, 0.05345F, 0.10823F),
localAngles = new Vector3(8.4958F, 176.5473F, 162.7601F),
localScale = new Vector3(0.0736F, 0.0736F, 0.0736F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.TreasureCache,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayKey"),
childName = "Chest",
localPos = new Vector3(0F, 0F, 0F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0F, 0F, 0F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.BossDamageBonus,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayAPRound"),
childName = "Chest",
localPos = new Vector3(0.1062F, 0.11162F, 0.09627F),
localAngles = new Vector3(90F, 41.5689F, 0F),
localScale = new Vector3(0.2279F, 0.2279F, 0.2279F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.SlowOnHit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBauble"),
childName = "Pelvis",
localPos = new Vector3(-0.0074F, 0.076F, -0.0864F),
localAngles = new Vector3(0F, 23.7651F, 0F),
localScale = new Vector3(0.0687F, 0.0687F, 0.0687F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ExtraLife,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayHippo"),
childName = "Chest",
localPos = new Vector3(-0.12967F, 0.38034F, -0.07946F),
localAngles = new Vector3(330.14F, 342.5552F, 1.57057F),
localScale = new Vector3(0.20914F, 0.20914F, 0.20241F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.KillEliteFrenzy,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBrainstalk"),
childName = "Chest",
localPos = new Vector3(0F, 0.21957F, -0.11009F),
localAngles = new Vector3(278.9036F, 0F, 0F),
localScale = new Vector3(0.2638F, 0.2638F, 0.2638F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.RepeatHeal,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayCorpseFlower"),
childName = "Chest",
localPos = new Vector3(0.07487F, 0.30183F, -0.24376F),
localAngles = new Vector3(297.8014F, 5.34769F, 336.6403F),
localScale = new Vector3(0.1511F, 0.1511F, 0.1511F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.AutoCastEquipment,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayFossil"),
childName = "HandR",
localPos = new Vector3(0.02033F, 0.08756F, -0.02643F),
localAngles = new Vector3(341.778F, 35.04337F, 99.35478F),
localScale = new Vector3(0.0722F, 0.0722F, 0.0722F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.IncreaseHealing,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayAntler"),
childName = "Head",
localPos = new Vector3(0.07899F, 0.04586F, -0.05757F),
localAngles = new Vector3(0F, 90F, 0F),
localScale = new Vector3(0.16854F, 0.16854F, 0.16854F),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayAntler"),
childName = "Head",
localPos = new Vector3(-0.07899F, 0.04586F, -0.05757F),
localAngles = new Vector3(0F, 90F, 0F),
localScale = new Vector3(0.16854F, 0.16854F, -0.16854F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.TitanGoldDuringTP,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayGoldHeart"),
childName = "Chest",
localPos = new Vector3(-0.12607F, 0.1224F, -0.1718F),
localAngles = new Vector3(308.4481F, 225.9465F, 333.6598F),
localScale = new Vector3(0.1191F, 0.1191F, 0.1191F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.SprintWisp,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBrokenMask"),
childName = "Chest",
localPos = new Vector3(0F, 0F, 0F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0F, 0F, 0F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.BarrierOnKill,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBrooch"),
childName = "HandR",
localPos = new Vector3(0.02152F, 0.09368F, 0.00294F),
localAngles = new Vector3(270.8865F, 15.31104F, 231.1227F),
localScale = new Vector3(0.08837F, 0.08837F, 0.08837F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.TPHealingNova,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayGlowFlower"),
childName = "Chest",
localPos = new Vector3(0.03903F, 0.20738F, 0.08794F),
localAngles = new Vector3(328.997F, 26.08873F, 0.92311F),
localScale = new Vector3(0.13058F, 0.13058F, 0.01305F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.LunarUtilityReplacement,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBirdFoot"),
childName = "Chest",
localPos = new Vector3(0.1135F, 0.23809F, -0.18159F),
localAngles = new Vector3(53.33944F, 239.3017F, 36.17928F),
localScale = new Vector3(0.2833F, 0.2833F, 0.2833F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Thorns,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayRazorwireLeft"),
childName = "UpperArmR",
localPos = new Vector3(0.21954F, 0.25103F, 0.17563F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.5F, 0.5F, 0.5F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.LunarPrimaryReplacement,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBirdEye"),
childName = "Head",
localPos = new Vector3(0.04355F, 0.1722F, -0.01725F),
localAngles = new Vector3(16.12343F, 180F, 180F),
localScale = new Vector3(0.15152F, 0.15152F, 0.15152F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.NovaOnLowHealth,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayJellyGuts"),
childName = "Chest",
localPos = new Vector3(-0.0484F, 0.10984F, 0.02595F),
localAngles = new Vector3(316.2306F, 45.1087F, 303.6165F),
localScale = new Vector3(-0.01575F, -0.01575F, -0.01575F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.LunarTrinket,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBeads"),
childName = "LowerArmL",
localPos = new Vector3(0.0348F, 0.31195F, 0.00935F),
localAngles = new Vector3(0F, 0F, 90F),
localScale = new Vector3(1F, 1F, 1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Plant,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayInterstellarDeskPlant"),
childName = "Chest",
localPos = new Vector3(0.17787F, 0.31456F, -0.13987F),
localAngles = new Vector3(323.3734F, 109.097F, 237.1637F),
localScale = new Vector3(0.0429F, 0.0429F, 0.0429F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Bear,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBear"),
childName = "Chest",
localPos = new Vector3(0F, 0.43227F, -0.17999F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.17815F, 0.17815F, 0.18152F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.DeathMark,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayDeathMark"),
childName = "Head",
localPos = new Vector3(0F, 0.14709F, -0.03841F),
localAngles = new Vector3(2.90161F, 0.00004F, -0.00004F),
localScale = new Vector3(-0.0375F, -0.0341F, -0.0464F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ExplodeOnDeath,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayWilloWisp"),
childName = "Pelvis",
localPos = new Vector3(0.13742F, 0.07936F, 0.01299F),
localAngles = new Vector3(0.05995F, 358.84F, 8.76334F),
localScale = new Vector3(0.0283F, 0.0283F, 0.0283F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Seed,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplaySeed"),
childName = "Head",
localPos = new Vector3(-0.10265F, 0.07407F, -0.0262F),
localAngles = new Vector3(31.54254F, 236.3528F, 91.58902F),
localScale = new Vector3(0.01154F, 0.01009F, 0.01009F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.SprintOutOfCombat,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayWhip"),
childName = "Pelvis",
localPos = new Vector3(0.13093F, -0.05603F, -0.00595F),
localAngles = new Vector3(359.9796F, 0.38747F, 18.12464F),
localScale = new Vector3(0.31015F, 0.31015F, 0.31015F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.CooldownOnCrit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplaySkull"),
childName = "HandR",
localPos = new Vector3(0.03084F, 0.09123F, -0.01059F),
localAngles = new Vector3(85.9473F, 80.3895F, 184.9944F),
localScale = new Vector3(0.01652F, 0.01652F, 0.01652F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Phasing,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayStealthkit"),
childName = "Chest",
localPos = new Vector3(-0.0063F, 0.14073F, 0.00698F),
localAngles = new Vector3(90F, 0F, 0F),
localScale = new Vector3(0.02868F, 0.04732F, 0.03156F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.PersonalShield,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayShieldGenerator"),
childName = "Chest",
localPos = new Vector3(0F, 0.27486F, 0.05085F),
localAngles = new Vector3(88.26055F, 232.4472F, 51.9133F),
localScale = new Vector3(0.1057F, 0.1057F, 0.1057F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ShockNearby,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayTeslaCoil"),
childName = "Chest",
localPos = new Vector3(0F, 0.30543F, -0.16814F),
localAngles = new Vector3(285.2071F, 2.45629F, 357.4169F),
localScale = new Vector3(0.24216F, 0.24216F, 0.24216F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ShieldOnly,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayShieldBug"),
childName = "Head",
localPos = new Vector3(0.05392F, 0.20212F, -0.01851F),
localAngles = new Vector3(348.1819F, 268.0985F, 0.3896F),
localScale = new Vector3(0.3521F, 0.3521F, 0.3521F),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayShieldBug"),
childName = "Head",
localPos = new Vector3(-0.05153F, 0.20892F, -0.01879F),
localAngles = new Vector3(11.8181F, 268.0985F, 359.6104F),
localScale = new Vector3(0.3521F, 0.3521F, -0.3521F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.AlienHead,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayAlienHead"),
childName = "Base",
localPos = new Vector3(0.10939F, 0.13417F, -0.04608F),
localAngles = new Vector3(284.1171F, 239.5653F, 260.8905F),
localScale = new Vector3(0.6701F, 0.6701F, 0.6701F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.HeadHunter,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplaySkullCrown"),
childName = "Pelvis",
localPos = new Vector3(-0.01582F, -0.0147F, -0.0275F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.4769F, 0.15897F, 0.15897F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.EnergizedOnEquipmentUse,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayWarHorn"),
childName = "Chest",
localPos = new Vector3(-0.12219F, 0.22746F, 0.05431F),
localAngles = new Vector3(319.4921F, 95.44549F, 180.451F),
localScale = new Vector3(0.2732F, 0.2732F, 0.2732F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.FlatHealth,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplaySteakCurved"),
childName = "Pelvis",
localPos = new Vector3(-0.1416F, 0.09794F, -0.09843F),
localAngles = new Vector3(351.9934F, 237.8542F, 86.18441F),
localScale = new Vector3(0.1245F, 0.1155F, 0.1155F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Tooth,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayToothMeshLarge"),
childName = "Chest",
localPos = new Vector3(0F, 0F, 0F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0F, 0F, 0F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Pearl,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayPearl"),
childName = "Head",
localPos = new Vector3(0F, 0.086F, -0.02121F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.2F, 0.2F, 0.2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.ShinyPearl,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayShinyPearl"),
childName = "Head",
localPos = new Vector3(0F, 0F, 0F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.2F, 0.2F, 0.2F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.BonusGoldPackOnKill,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayTome"),
childName = "Base",
localPos = new Vector3(0.14954F, 0.05886F, 0.01124F),
localAngles = new Vector3(345.828F, 90.44725F, 85.56704F),
localScale = new Vector3(0.05585F, 0.05585F, 0.05585F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Squid,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplaySquidTurret"),
childName = "Chest",
localPos = new Vector3(0.08975F, 0.16036F, -0.16564F),
localAngles = new Vector3(5.65006F, 75.07253F, 276.3981F),
localScale = new Vector3(0.02F, 0.02F, 0.02F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Icicle,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayFrostRelic"),
childName = "Base",
localPos = new Vector3(0.45946F, 0.68513F, -0.43309F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(1F, 1F, 1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Talisman,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayTalisman"),
childName = "Base",
localPos = new Vector3(0.2F, 0.95856F, -0.46796F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(1F, 1F, 1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.LaserTurbine,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayLaserTurbine"),
childName = "Chest",
localPos = new Vector3(0F, 0.00283F, -0.24214F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.2159F, 0.2159F, 0.2159F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.FocusConvergence,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayFocusedConvergence"),
childName = "Base",
localPos = new Vector3(-0.5429F, 0.96699F, -0.33138F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.1F, 0.1F, 0.1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Incubator,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayAncestralIncubator"),
childName = "Chest",
localPos = new Vector3(-0.08381F, 0.17971F, -0.09568F),
localAngles = new Vector3(353.0521F, 317.2421F, 69.6292F),
localScale = new Vector3(0.02609F, 0.02609F, 0.02609F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.FireballsOnHit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayFireballsOnHit"),
childName = "Pelvis",
localPos = new Vector3(0.11671F, 0.06175F, 0.03695F),
localAngles = new Vector3(57.4526F, 80.80998F, 26.94625F),
localScale = new Vector3(0.00805F, 0.00805F, 0.00805F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.SiphonOnLowHealth,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplaySiphonOnLowHealth"),
childName = "Chest",
localPos = new Vector3(0F, 0F, 0F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0F, 0F, 0F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.BleedOnHitAndExplode,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBleedOnHitAndExplode"),
childName = "Head",
localPos = new Vector3(0.00576F, 0.04279F, 0.0169F),
localAngles = new Vector3(326.6455F, -0.00001F, 0.00003F),
localScale = new Vector3(0.0486F, 0.0486F, 0.0486F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.MonstersOnShrineUse,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayMonstersOnShrineUse"),
childName = "Chest",
localPos = new Vector3(0.0022F, 0.084F, 0.066F),
localAngles = new Vector3(352.4521F, 260.6884F, 341.5106F),
localScale = new Vector3(-0.01067F, -0.01067F, -0.01067F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.RandomDamageZone,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayRandomDamageZone"),
childName = "Chest",
localPos = new Vector3(0F, 0.20552F, 0.03736F),
localAngles = new Vector3(349.218F, 235.9453F, 0F),
localScale = new Vector3(0.00365F, 0.00365F, 0.00365F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Fruit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayFruit"),
childName = "Chest",
localPos = new Vector3(-0.08468F, 0.06027F, -0.05945F),
localAngles = new Vector3(355.5085F, 54.78192F, 339.6581F),
localScale = new Vector3(0.2118F, 0.2118F, 0.2118F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.AffixRed,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteHorn"),
childName = "Chest",
localPos = new Vector3(0F, 0F, 0F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.01F, 0.01F, 0.01F),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteHorn"),
childName = "Chest",
localPos = new Vector3(0F, 0F, 0F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.01F, 0.01F, 0.01F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.AffixBlue,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteRhinoHorn"),
childName = "Chest",
localPos = new Vector3(0F, 0F, 0F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.01F, 0.01F, 0.01F),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteRhinoHorn"),
childName = "Chest",
localPos = new Vector3(0F, 0F, 0F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.01F, 0.01F, 0.01F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.AffixWhite,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteIceCrown"),
childName = "Chest",
localPos = new Vector3(0F, 0F, 0F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.01F, 0.01F, 0.01F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.AffixPoison,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteUrchinCrown"),
childName = "Chest",
localPos = new Vector3(0F, 0F, 0F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.01F, 0.01F, 0.01F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.AffixHaunted,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayEliteStealthCrown"),
childName = "Chest",
localPos = new Vector3(0F, 0F, 0F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.01F, 0.01F, 0.01F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.CritOnUse,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayNeuralImplant"),
childName = "Head",
localPos = new Vector3(0F, 0.24193F, -0.00185F),
localAngles = new Vector3(272.112F, 180.0012F, 179.9989F),
localScale = new Vector3(0.13259F, 0.13259F, 0.13259F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.DroneBackup,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayRadio"),
childName = "Pelvis",
localPos = new Vector3(0F, 0.1269F, 0F),
localAngles = new Vector3(0F, 90F, 0F),
localScale = new Vector3(0.01F, 0.01F, 0.01F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Lightning,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayLightningArmRight"),
childName = "UpperArmR",
localPos = new Vector3(0F, 0F, 0F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.3413F, 0.3413F, 0.3413F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.BurnNearby,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayPotion"),
childName = "Chest",
localPos = new Vector3(0F, 0F, 0F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.01F, 0.01F, 0.01F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.CrippleWard,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayEffigy"),
childName = "Chest",
localPos = new Vector3(0F, 0F, 0F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.01F, 0.01F, 0.01F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.QuestVolatileBattery,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBatteryArray"),
childName = "Chest",
localPos = new Vector3(0F, 0.2584F, -0.0987F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.2188F, 0.2188F, 0.2188F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.GainArmor,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayElephantFigure"),
childName = "Chest",
localPos = new Vector3(0F, 0F, 0F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.01F, 0.01F, 0.01F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Recycle,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayRecycler"),
childName = "Chest",
localPos = new Vector3(0F, 0F, 0F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.01F, 0.01F, 0.01F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.FireBallDash,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayEgg"),
childName = "Pelvis",
localPos = new Vector3(0F, 0.1269F, 0F),
localAngles = new Vector3(0F, 90F, 0F),
localScale = new Vector3(0.01F, 0.01F, 0.01F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Cleanse,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayWaterPack"),
childName = "Chest",
localPos = new Vector3(0F, 0F, 0F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.01F, 0.01F, 0.01F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Tonic,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayTonic"),
childName = "Chest",
localPos = new Vector3(0F, 0F, 0F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.01F, 0.01F, 0.01F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Gateway,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayVase"),
childName = "Chest",
localPos = new Vector3(0F, 0F, 0F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.01F, 0.01F, 0.01F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Meteor,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayMeteor"),
childName = "Base",
localPos = new Vector3(0F, 1.18521F, 0F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(1F, 1F, 1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Saw,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplaySawmerang"),
childName = "Base",
localPos = new Vector3(0F, 0.678F, -0.46362F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(1F, 1F, 1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Blackhole,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayGravCube"),
childName = "Base",
localPos = new Vector3(-0.2F, 0.94981F, -0.46657F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.5F, 0.5F, 0.5F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Scanner,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayScanner"),
childName = "Chest",
localPos = new Vector3(0F, 0F, 0F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.01F, 0.01F, 0.01F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.DeathProjectile,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayDeathProjectile"),
childName = "Pelvis",
localPos = new Vector3(0F, 0.028F, -0.0977F),
localAngles = new Vector3(0F, 180F, 0F),
localScale = new Vector3(0.0596F, 0.0596F, 0.0596F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.LifestealOnHit,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayLifestealOnHit"),
childName = "Head",
localPos = new Vector3(0F, 0.07288F, 0.00119F),
localAngles = new Vector3(274.4367F, 270F, 90F),
localScale = new Vector3(0.1F, 0.1F, 0.1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.TeamWarCry,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayTeamWarCry"),
childName = "Chest",
localPos = new Vector3(0F, 0F, 0F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.01F, 0.01F, 0.01F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });
            #endregion

            //itemDisplayRuleSet.keyAssetRuleGroups = itemDisplayRules.ToArray();
            itemDisplayRuleSet.GenerateRuntimeValues();
        }

        private static CharacterModel.RendererInfo[] SkinRendererInfos(CharacterModel.RendererInfo[] defaultRenderers, Material[] materials)
        {
            CharacterModel.RendererInfo[] newRendererInfos = new CharacterModel.RendererInfo[defaultRenderers.Length];
            defaultRenderers.CopyTo(newRendererInfos, 0);

            newRendererInfos[0].defaultMaterial = materials[0];
            newRendererInfos[1].defaultMaterial = materials[1];
            newRendererInfos[instance.mainRendererIndex].defaultMaterial = materials[2];

            return newRendererInfos;
        }
    }
}