using BepInEx;
using RiskOfRuinaMod.Modules.Survivors;
using R2API.Utils;
using RoR2;
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;
using UnityEngine;
using RiskOfRuinaMod.Modules;
using System.Runtime.CompilerServices;
using RoR2.Skills;
using UnityEngine.Networking;
using RoR2.Projectile;
using RiskOfRuinaMod.Modules.Components;
using System.Collections.ObjectModel;
using RoR2.Networking;
using RoR2.UI;
using Unity.Jobs;
using R2API;
using RiskOfRuinaMod.Modules.Items;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace RiskOfRuinaMod
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(MODUID, MODNAME, MODVERSION)]
    [R2APISubmoduleDependency(new string[]
    {
        "PrefabAPI",
        "LanguageAPI",
        "SoundAPI",
        "ItemAPI",
        "ItemDropAPI",
        "ResourcesAPI",
    })]

    public class RiskOfRuinaPlugin : BaseUnityPlugin
    {
        // if you don't change these you're giving permission to deprecate the mod-
        //  please change the names to your own stuff, thanks
        //   this shouldn't even have to be said
        public const string MODUID = "com.Scoops.RiskOfRuina";
        public const string MODNAME = "RiskOfRuina";
        public const string MODVERSION = "1.0.0";

        // a prefix for name tokens to prevent conflicts- please capitalize all name tokens for convention
        public const string developerPrefix = "COF";

        public static bool DEBUG_MODE = true;

        // Networking stuff
        public static GameObject CentralNetworkObject;
        public static GameObject _centralNetworkObjectSpawned;

        internal List<SurvivorBase> Survivors = new List<SurvivorBase>();
        public static GameObject characterPrefab;

        public static RiskOfRuinaPlugin instance;

        public static bool ancientScepterInstalled = false;

        public static uint argaliaSkinIndex = 1;
        public bool IsRedMistSelected = false;
        public bool IsArbiterSelected = false;
        public bool IsModCharSelected = false;
        public string CurrentCharacterNameSelected = "";
        public bool songOverride = false;
        public uint currentOverrideSong;

        private Modules.DoTCore dotCore;
        private Modules.ItemManager itemManager;

        private void Awake()
        {
            instance = this;

            // load assets and read config
            Modules.Assets.Initialize();
            Modules.Config.ReadConfig();
            Modules.CameraParams.InitializeParams(); // create camera params for our character to use
            Modules.States.RegisterStates(); // register states for networking
            Modules.Buffs.RegisterBuffs(); // add and register custom buffs/debuffs
            dotCore = new Modules.DoTCore();
            Modules.Projectiles.RegisterProjectiles(); // add and register custom projectiles
            Modules.Tokens.AddTokens(); // register name tokens
            Modules.Music.Initialize(); // register Music events

            this.itemManager = new ItemManager();
            this.itemManager.items.Add(new ArbitersTrophy());
            this.itemManager.items.Add(new BlackTea());
            this.itemManager.items.Add(new Prescript());
            this.itemManager.items.Add(new LiuBadge());
            this.itemManager.items.Add(new WorkshopAmmo());
            this.itemManager.items.Add(new MoonlightStone());
            ItemManager.instance.AddItems(); // register items

            Modules.ItemDisplays.PopulateDisplays(); // collect item display prefabs for use in our display rules

            // survivor initialization
            new Modules.Survivors.RedMist().Initialize();
            new Modules.Survivors.AnArbiter().Initialize();

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.DestroyedClone.AncientScepter"))
            {
                ancientScepterInstalled = true;
                Modules.Skills.ScepterSkillSetup(developerPrefix);
                ScepterSetup();
            }

            var tmpGo = new GameObject("tmpGo");
            tmpGo.AddComponent<NetworkIdentity>();
            CentralNetworkObject = tmpGo.InstantiateClone("riskOfRuinaNetworkManager");
            GameObject.Destroy(tmpGo);
            CentralNetworkObject.AddComponent<RiskOfRuinaNetworkManager>();

            // now make a content pack and add it- this part will change with the next update
            new Modules.ContentPacks().Initialize();

            RoR2.ContentManagement.ContentManager.onContentPacksAssigned += LateSetup;

            Hook();
        }

        private void LateSetup(HG.ReadOnlyArray<RoR2.ContentManagement.ReadOnlyContentPack> obj)
        {
            // have to set item displays later now because they require direct object references..
            Modules.Survivors.RedMist.instance.SetItemDisplays();
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void ScepterSetup()
        {
            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(Skills.ScepterBasicAttack, "RedMistBody", SkillSlot.Primary, 0);
            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(Skills.scepterShockwaveDef, "ArbiterBody", SkillSlot.Special, 0);
        }

        private void Hook()
        {
            if (Modules.Config.snapLevel.Value)
            {
                On.RoR2.CharacterBody.OnLevelUp += CharacterBody_OnLevelUp;
            }
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEvent_OnHitEnemy;
            On.RoR2.GlobalEventManager.OnCharacterDeath += GlobalEvent_OnCharacterDeath;
            On.RoR2.CharacterSpeech.BrotherSpeechDriver.DoInitialSightResponse += BrotherSpeechDriver_DoInitialSightResponse;
            On.RoR2.CharacterSpeech.BrotherSpeechDriver.OnBodyKill += BrotherSpeechDriver_OnBodyKill;
            On.RoR2.Networking.GameNetworkManager.OnClientSceneChanged += new On.RoR2.Networking.GameNetworkManager.hook_OnClientSceneChanged(this.GameNetworkManager_OnClientSceneChanged_Hook);
            On.RoR2.UI.CharacterSelectController.SelectSurvivor += new On.RoR2.UI.CharacterSelectController.hook_SelectSurvivor(this.OnSurvivorSelected_Hook);
            On.RoR2.CharacterBody.OnSkillActivated += CharacterBody_OnSkillActivated;
            DotController.onDotInflictedServerGlobal += new DotController.OnDotInflictedServerGlobalDelegate(DotController_InflictDot);
        }

        private void CharacterBody_OnSkillActivated(On.RoR2.CharacterBody.orig_OnSkillActivated orig, CharacterBody self, GenericSkill skill)
        {
            orig(self, skill);

            // Fairy? Nuke em
            if (self && NetworkServer.active)
            {
                DotController selfDotController = DotController.FindDotController(self.gameObject);

                if (selfDotController && selfDotController.HasDotActive(Modules.DoTCore.FairyIndex))
                {
                    int stacks = self.GetBuffCount(Modules.Buffs.fairyDebuff);
                    int stacksToRemove = (int)(stacks / 3);
                    int stacksRemoved = 0;
                    for (int i = 0; i < selfDotController.dotStackList.Count; i++)
                    {
                        DotController.DotStack stack = selfDotController.dotStackList[i];
                        if (stack.dotIndex == Modules.DoTCore.FairyIndex)
                        {
                            DamageInfo fairyDamage = new DamageInfo()
                            {
                                attacker = stack.attackerObject,
                                inflictor = stack.attackerObject,
                                crit = false,
                                damage = stack.attackerObject.GetComponent<CharacterBody>().damage * Modules.StaticValues.fairyDebuffCoefficient,
                                position = self.corePosition,
                                force = UnityEngine.Vector3.zero,
                                damageType = RoR2.DamageType.BypassArmor,
                                damageColorIndex = RoR2.DamageColorIndex.Bleed,
                                dotIndex = Modules.DoTCore.FairyIndex,
                                procCoefficient = 1f
                            };

                            self.healthComponent.TakeDamage(fairyDamage);

                            GlobalEventManager.instance.OnHitEnemy(fairyDamage, self.healthComponent.body.gameObject);
                            GlobalEventManager.instance.OnHitAll(fairyDamage, self.healthComponent.body.gameObject);
                        }
                    }

                    for (int i = selfDotController.dotStackList.Count - 1; i >= 0; i--)
                    {
                        DotController.DotStack stack = selfDotController.dotStackList[i];
                        if (stack.dotIndex == Modules.DoTCore.FairyIndex)
                        {
                            if (stacksRemoved < stacksToRemove)
                            {
                                selfDotController.RemoveDotStackAtServer(i);
                                stacksRemoved++;
                            }
                        }
                    }

                    EffectData effectData = new EffectData();
                    effectData.rotation = Util.QuaternionSafeLookRotation(UnityEngine.Vector3.zero);
                    effectData.origin = self.corePosition;

                    EffectManager.SpawnEffect(Modules.Assets.fairyProcEffect, effectData, false);
                }
            }
        }

        private void OnSurvivorSelected_Hook(On.RoR2.UI.CharacterSelectController.orig_SelectSurvivor orig, CharacterSelectController self, SurvivorIndex survivor)
        {
            orig.Invoke(self, survivor);
            SurvivorDef survivorDef = SurvivorCatalog.GetSurvivorDef(survivor);
            if (survivorDef != null)
            {
                this.IsRedMistSelected = survivorDef.cachedName == "RedMist";
                this.IsArbiterSelected = survivorDef.cachedName == "Arbiter";
                this.IsModCharSelected = IsArbiterSelected || IsRedMistSelected;
            }
            this.CurrentCharacterNameSelected = survivorDef.cachedName;
        }

        private void GameNetworkManager_OnClientSceneChanged_Hook(On.RoR2.Networking.GameNetworkManager.orig_OnClientSceneChanged orig, GameNetworkManager self, NetworkConnection conn)
        {
            orig.Invoke(self, conn);
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("outro") && this.IsModCharSelected)
            {
                songOverride = true;
                currentOverrideSong = Util.PlaySound("Play_Dark_Fantasy_Studio___Sun_and_Moon", base.gameObject);
                Music.musicSources++;
            } else if (songOverride)
            {
                songOverride = false;
                AkSoundEngine.StopPlayingID(currentOverrideSong);
                Music.musicSources--;
            }
        }

        private void BrotherSpeechDriver_DoInitialSightResponse(On.RoR2.CharacterSpeech.BrotherSpeechDriver.orig_DoInitialSightResponse orig, RoR2.CharacterSpeech.BrotherSpeechDriver self)
        {
            bool isThereARedMist = false;

            ReadOnlyCollection<CharacterBody> characterBodies = CharacterBody.readOnlyInstancesList;
            for (int i = 0; i < characterBodies.Count; i++)
            {
                BodyIndex bodyIndex = characterBodies[i].bodyIndex;
                isThereARedMist |= (bodyIndex == BodyCatalog.FindBodyIndex(RedMist.redMistPrefab));
            }

            if (isThereARedMist)
            {
                RoR2.CharacterSpeech.CharacterSpeechController.SpeechInfo[] responsePool = new RoR2.CharacterSpeech.CharacterSpeechController.SpeechInfo[]
                {
                    new RoR2.CharacterSpeech.CharacterSpeechController.SpeechInfo
                    {
                        duration = 1f,
                        maxWait = 4f,
                        mustPlay = true,
                        priority = 0f,
                        token = "BROTHER_SEE_REDMIST_1"
                    }
                };

                self.SendReponseFromPool(responsePool);
            }

            orig(self);
        }

        private void BrotherSpeechDriver_OnBodyKill(On.RoR2.CharacterSpeech.BrotherSpeechDriver.orig_OnBodyKill orig, RoR2.CharacterSpeech.BrotherSpeechDriver self, DamageReport damageReport)
        {
            if (damageReport.victimBody)
            {
                if (damageReport.victimBodyIndex == BodyCatalog.FindBodyIndex(RedMist.redMistPrefab))
                {
                    RoR2.CharacterSpeech.CharacterSpeechController.SpeechInfo[] responsePool = new RoR2.CharacterSpeech.CharacterSpeechController.SpeechInfo[]
                    {
                    new RoR2.CharacterSpeech.CharacterSpeechController.SpeechInfo
                    {
                        duration = 1f,
                        maxWait = 4f,
                        mustPlay = true,
                        priority = 0f,
                        token = "BROTHER_KILL_REDMIST_1"
                    }
                    };

                    self.SendReponseFromPool(responsePool);
                }
            }

            orig(self, damageReport);
        }

        private void CharacterBody_OnLevelUp(On.RoR2.CharacterBody.orig_OnLevelUp orig, CharacterBody self)
        {
            orig(self);

            if (self)
            {
                if (this.IsModCharSelected)
                {
                    Util.PlaySound("Ruina_Snap", self.gameObject);
                }
            }
        }

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);

            if (self)
            {
                if (self.HasBuff(Buffs.feebleDebuff))
                {
                    self.armor *= (1 - StaticValues.feebleArmorDamageAmount);
                    self.damage *= (1 - StaticValues.feebleDamageDamageAmount);
                }

                if (self.HasBuff(Buffs.warpBuff))
                {
                    self.moveSpeed *= 2;
                    self.attackSpeed *= 2;
                }

                if (self.HasBuff(Buffs.strengthBuff))
                {
                    if (self.isPlayerControlled)
                    {
                        self.moveSpeed *= 1.5f;
                        self.attackSpeed *= 1.5f;
                        self.regen *= 2f;
                        self.armor += 50f;
                        self.damage *= 1.5f;
                    } else
                    {
                        self.moveSpeed *= 1.5f;
                        self.attackSpeed *= 5f;
                        self.regen *= 5f;
                        self.armor += 100f;
                        self.damage *= 5f;
                    }

                    if (self.skillLocator)
                    {
                        if (self.skillLocator.primary) self.skillLocator.primary.cooldownScale -= 0.25f;
                        if (self.skillLocator.secondary) self.skillLocator.secondary.cooldownScale -= 0.25f;
                        if (self.skillLocator.utility) self.skillLocator.utility.cooldownScale -= 0.25f;
                        if (self.skillLocator.special) self.skillLocator.special.cooldownScale -= 0.25f;
                    }
                }

                RedMistStatTracker statTracker = self.GetComponent<RedMistStatTracker>();
                RedMistEmotionComponent emoComponent = self.GetComponent<RedMistEmotionComponent>();
                if (statTracker && emoComponent)
                {
                    self.moveSpeed = statTracker.CalculateMoveSpeed(self.moveSpeed);
                    self.attackSpeed = statTracker.CalculateAttackSpeed(self.attackSpeed);

                    float sprintBonus = 0f;
                    float OOCsprintBonus = 0f;

                    if (self.inventory && self.inventory.GetItemCount(RoR2Content.Items.SprintBonus) > 0)
                    {
                        sprintBonus += Modules.Config.sprintSpeedMult.Value * (float)self.inventory.GetItemCount(RoR2Content.Items.SprintBonus);
                    }

                    if (self.inventory && self.inventory.GetItemCount(RoR2Content.Items.SprintOutOfCombat) > 0)
                    {
                        OOCsprintBonus += (Modules.Config.sprintSpeedMult.Value * 2f) * (float)self.inventory.GetItemCount(RoR2Content.Items.SprintOutOfCombat);
                    }

                    if (self.HasBuff(Modules.Buffs.EGOBuff))
                    {
                        if (!emoComponent.inEGO && NetworkServer.active)
                        {
                            self.RemoveBuff(Buffs.EGOBuff);
                        }
                        self.armor += 50f;
                        self.moveSpeed += 2f;
                        self.sprintingSpeedMultiplier = 2.2f;
                    }
                    else
                    {
                        self.sprintingSpeedMultiplier = 1.6f;
                    }

                    self.sprintingSpeedMultiplier += sprintBonus;

                    if (self.outOfCombat)
                    {
                        self.sprintingSpeedMultiplier += OOCsprintBonus;
                    }

                    if (self.isSprinting)
                    {
                        self.moveSpeed = self.moveSpeed * (self.sprintingSpeedMultiplier);
                    }
                }
            }
        }

        private void GlobalEvent_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, UnityEngine.GameObject victim)
        {
            GameObject attacker = damageInfo.attacker;

            if (self && attacker)
            {
                CharacterBody component = attacker.GetComponent<CharacterBody>();
                CharacterBody component2 = victim.GetComponent<CharacterBody>();
                if (component.teamComponent.teamIndex != component2.teamComponent.teamIndex)
                {
                    if (component.baseNameToken == RiskOfRuinaPlugin.developerPrefix + "_REDMIST_BODY_NAME")
                    {
                        RedMistStatTracker statTracker = component.GetComponent<RedMistStatTracker>();
                        RedMistEmotionComponent emoTracker = component.GetComponent<RedMistEmotionComponent>();
                        if (statTracker && emoTracker)
                        {
                            float trueDamage = component.damage + (statTracker.DifferenceAttackSpeed * Modules.Config.attackSpeedMult.Value) + (statTracker.DifferenceMoveSpeed * Modules.Config.moveSpeedMult.Value);
                            float calcCoefficient = Mathf.Clamp(damageInfo.damage / trueDamage, 0f, StaticValues.onrushDamageCoefficient);
                            float calcMult = (((float)RoR2.Run.instance.stageClearCount) / ((float)RoR2.Run.instance.stageClearCount + 1f));
                            float emotion = calcCoefficient * Modules.Config.emotionRatio.Value;
                            
                            float enemyValueMult = 1f;
                            if (component2.isElite) enemyValueMult = 1.2f;
                            if (component2.isBoss) enemyValueMult = 1.4f;

                            emoTracker.AddEmotion((emotion + (emotion * calcMult)) * enemyValueMult);
                        }
                    }

                    if (component.baseNameToken == RiskOfRuinaPlugin.developerPrefix + "_ARBITER_BODY_NAME")
                    {
                        if (damageInfo.dotIndex != Modules.DoTCore.FairyIndex && Util.CheckRoll(100f * damageInfo.procCoefficient, component.master))
                        {
                            InflictDotInfo inflictDotInfo = default(InflictDotInfo);
                            inflictDotInfo.attackerObject = damageInfo.attacker;
                            inflictDotInfo.victimObject = victim;
                            inflictDotInfo.dotIndex = Modules.DoTCore.FairyIndex;
                            inflictDotInfo.duration = 20f;
                            inflictDotInfo.damageMultiplier = 0;
                            InflictDotInfo inflictDotInfo2 = inflictDotInfo;
                            DotController.InflictDot(ref inflictDotInfo2);
                        }
                    }
                }
            }

            orig(self, damageInfo, victim);
        }

        private void GlobalEvent_OnCharacterDeath(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)
        {
            if ((damageReport != null) ? damageReport.attackerBody : null)
            {
                if (damageReport.attackerBody.baseNameToken == RiskOfRuinaPlugin.developerPrefix + "_REDMIST_BODY_NAME")
                {
                    RedMistEmotionComponent emoTracker = damageReport.attackerBody.GetComponent<RedMistEmotionComponent>();
                    if (emoTracker && emoTracker.inEGO)
                    {
                        if (NetworkServer.active) damageReport.attackerBody.AddBuff(Modules.Buffs.RedMistBuff);
                    }

                    if (damageReport.combinedHealthBeforeDamage - damageReport.damageDealt <= -damageReport.victim.fullHealth)
                    {
                        EffectData effectData = new EffectData();
                        effectData.origin = damageReport.victimBody.corePosition;
                        effectData.scale = 1;

                        EffectManager.SpawnEffect(Modules.Assets.mistEffect, effectData, true);

                        if (damageReport.victimBody.modelLocator && damageReport.victimBody.modelLocator.modelTransform.GetComponent<CharacterModel>())
                        {
                            if (NetworkServer.active)
                            {
                                RiskOfRuinaNetworkManager.SetInvisible(damageReport.victimBody.gameObject);
                            }
                        }
                    }
                }

                if (damageReport.victimBody.baseNameToken == RiskOfRuinaPlugin.developerPrefix + "_REDMIST_BODY_NAME" || damageReport.victimBody.baseNameToken == RiskOfRuinaPlugin.developerPrefix + "_ARBITER_BODY_NAME")
                {
                    EffectData effectData = new EffectData();
                    effectData.origin = damageReport.victimBody.corePosition;
                    effectData.scale = 1;
                    EffectManager.SpawnEffect(Assets.pagePoof, effectData, true);
                    if (NetworkServer.active)
                    {
                        RiskOfRuinaNetworkManager.SetInvisible(damageReport.victimBody.gameObject);
                    }
                }
            }
            orig.Invoke(self, damageReport);
        }

        private void DotController_InflictDot(RoR2.DotController self, ref RoR2.InflictDotInfo dotInfo)
        {
            if (dotInfo.dotIndex == Modules.DoTCore.FairyIndex)
            {
                int i = 0;
                int count = self.dotStackList.Count;
                while (i < count)
                {
                    if (self.dotStackList[i].dotIndex == Modules.DoTCore.FairyIndex)
                    {
                        self.dotStackList[i].timer = Mathf.Max(self.dotStackList[i].timer, dotInfo.duration);
                    }
                    i++;
                }
            }
        }
    }
}