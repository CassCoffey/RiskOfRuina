using System.Reflection;
using R2API;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using System.IO;
using System.Collections.Generic;
using RoR2.UI;

namespace RiskOfRuinaMod.Modules
{
    internal static class Assets
    {
        // the assetbundle to load assets from
        internal static AssetBundle mainAssetBundle;

        internal static GameObject trackerPrefab;

        // objects
        internal static GameObject pillarObject;

        // items
        internal static GameObject arbiterTrophy;
        internal static GameObject blackTea;
        internal static GameObject backwardsClock;
        internal static GameObject moonlightStone;
        internal static GameObject prescript;
        internal static GameObject liuBadge;
        internal static GameObject udjatMask;
        internal static GameObject workshopAmmo;
        internal static GameObject weddingRing;
        internal static GameObject reverberation;

        // projectile ghosts
        internal static GameObject fairyTrail;
        internal static GameObject pillarSpear;

        // particle effects
        internal static GameObject swordHitEffect;
        internal static GameObject argaliaSwordHitEffect;
        internal static GameObject blockEffect;
        internal static GameObject mistEffect;

        internal static GameObject swordSwingEffect;
        internal static GameObject EGOSwordSwingEffect;
        internal static GameObject spearPierceEffect;
        internal static GameObject EGOSpearPierceEffect;
        internal static GameObject HorizontalSwordSwingEffect;
        internal static GameObject groundPoundEffect;
        internal static GameObject swordSpinEffect;
        internal static GameObject swordSpinEffectTwo;
        internal static GameObject counterBurst;

        internal static GameObject argaliaSwordSwingEffect;
        internal static GameObject argaliaEGOSwordSwingEffect;
        internal static GameObject argaliaSpearPierceEffect;
        internal static GameObject argaliaEGOSpearPierceEffect;
        internal static GameObject argaliaHorizontalSwordSwingEffect;
        internal static GameObject argaliaGroundPoundEffect;
        internal static GameObject argaliaSwordSpinEffect;
        internal static GameObject argaliaSwordSpinEffectTwo;
        internal static GameObject argaliaCounterBurst;

        internal static GameObject phaseEffect;
        internal static GameObject argaliaPhaseEffect;
        internal static GameObject phaseMistEffect;

        internal static GameObject afterimageSlash;
        internal static GameObject afterimageBlock;
        internal static GameObject argaliaAfterimageSlash;
        internal static GameObject argaliaAfterimageBlock;

        internal static GameObject EGOActivate;
        internal static GameObject argaliaEGOActivate;
        internal static GameObject EGODeactivate;

        internal static GameObject fairyProcEffect;
        internal static GameObject fairyExplodeEffect;
        internal static GameObject fairyDeleteEffect;
        internal static GameObject fairyHitEffect;

        internal static GameObject lockEffect5s;
        internal static GameObject lockEffect4s;
        internal static GameObject lockEffect3s;
        internal static GameObject lockEffect2s;
        internal static GameObject lockEffect1s;
        internal static GameObject lockEffectBreak;

        internal static GameObject unlockEffect;

        internal static GameObject pillarImpactEffect;

        internal static GameObject shockwaveEffect;

        internal static GameObject armSwingEffect;

        internal static GameObject pagePoof;

        // networked hit sounds
        internal static NetworkSoundEventDef swordHitSoundVert;
        internal static NetworkSoundEventDef swordHitSoundHori;
        internal static NetworkSoundEventDef swordHitSoundStab;
        internal static NetworkSoundEventDef swordHitEGOSoundVert;
        internal static NetworkSoundEventDef swordHitEGOSoundHori;
        internal static NetworkSoundEventDef swordHitEGOSoundStab;
        internal static NetworkSoundEventDef swordHitEGOSoundGRHorizontal;
        internal static NetworkSoundEventDef fairyHitSound;

        // lists of assets to add to contentpack
        internal static List<NetworkSoundEventDef> networkSoundEventDefs = new List<NetworkSoundEventDef>();
        internal static List<EffectDef> effectDefs = new List<EffectDef>();

        // cache these and use to create our own materials
        internal static Shader hotpoo = Resources.Load<Shader>("Shaders/Deferred/HGStandard");
        internal static Material commandoMat;
        private static string[] assetNames = new string[0];

        // CHANGE THIS
        private const string assetbundleName = "ruinabundle";

        internal static void Initialize()
        {
            if (assetbundleName == "myassetbundle")
            {
                Debug.LogError("AssetBundle name hasn't been changed- not loading any assets to avoid conflicts");
                return;
            }

            LoadAssetBundle();
            LoadSoundbank();
            PopulateAssets();
        }

        internal static void LoadAssetBundle()
        {
            if (mainAssetBundle == null)
            {
                using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("RiskOfRuinaMod." + assetbundleName))
                {
                    mainAssetBundle = AssetBundle.LoadFromStream(assetStream);
                }
            }

            assetNames = mainAssetBundle.GetAllAssetNames();
        }

        internal static void LoadSoundbank()
        {
            using (Stream manifestResourceStream2 = Assembly.GetExecutingAssembly().GetManifestResourceStream("RiskOfRuinaMod.ruina.bnk"))
            {
                byte[] array = new byte[manifestResourceStream2.Length];
                manifestResourceStream2.Read(array, 0, array.Length);
                SoundAPI.SoundBanks.Add(array);
            }
        }

        internal static void PopulateAssets()
        {
            if (!mainAssetBundle)
            {
                Debug.LogError("There is no AssetBundle to load assets from.");
                return;
            }

            // Generic Assets

            pagePoof = LoadEffect("PagesExplosion", "Play_Battle_Dead");

            Assets.trackerPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/HuntressTrackingIndicator"), "RedMistTrackerPrefab", false);
            Assets.trackerPrefab.transform.Find("Core Pip").gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            Assets.trackerPrefab.transform.Find("Core Pip").localScale = new Vector3(0.15f, 0.15f, 0.15f);
            Assets.trackerPrefab.transform.Find("Core, Dark").gameObject.GetComponent<SpriteRenderer>().color = Color.black;
            Assets.trackerPrefab.transform.Find("Core, Dark").localScale = new Vector3(0.1f, 0.1f, 0.1f);
            foreach (SpriteRenderer spriteRenderer in Assets.trackerPrefab.transform.Find("Holder").gameObject.GetComponentsInChildren<SpriteRenderer>())
            {
                if (spriteRenderer)
                {
                    spriteRenderer.gameObject.transform.localScale = new Vector3(0.2f, 0.2f, 1f);
                    spriteRenderer.color = Color.white;
                }
            }

            arbiterTrophy = mainAssetBundle.LoadAsset<GameObject>("mdlRedMistArm");
            liuBadge = mainAssetBundle.LoadAsset<GameObject>("mdlLiuBadge");
            moonlightStone = mainAssetBundle.LoadAsset<GameObject>("mdlMoonlightStone");
            prescript = mainAssetBundle.LoadAsset<GameObject>("mdlPrescript");
            blackTea = mainAssetBundle.LoadAsset<GameObject>("mdlTeaCup");
            udjatMask = mainAssetBundle.LoadAsset<GameObject>("mdlUdjatMask");
            workshopAmmo = mainAssetBundle.LoadAsset<GameObject>("mdlWorkshopAmmo");
            weddingRing = mainAssetBundle.LoadAsset<GameObject>("mdlWeddingRing");
            backwardsClock = mainAssetBundle.LoadAsset<GameObject>("mdlBackwardsClock");
            reverberation = mainAssetBundle.LoadAsset<GameObject>("mdlReverberation");


            // Red Mist Assets

            swordSwingEffect = LoadEffect("RedMistSwordSwing", true);
            EGOSwordSwingEffect = LoadEffect("RedMistEGOSwordSwing", true);
            spearPierceEffect = LoadEffect("RedMistSpearPierce", true);
            EGOSpearPierceEffect = LoadEffect("RedMistEGOSpearPierce", true);
            HorizontalSwordSwingEffect = LoadEffect("RedMistHorizontalSwordSwing", true);
            groundPoundEffect = LoadEffect("RedMistGroundPound");
            swordSpinEffect = LoadEffect("RedMistSpin", true);
            swordSpinEffectTwo = LoadEffect("RedMistSpinTwo", true);
            mistEffect = LoadEffect("MistEffect");
            swordHitEffect = LoadEffect("SwordHitEffect");
            blockEffect = LoadEffect("BlockEffect");
            counterBurst = LoadEffect("CounterBurst");

            argaliaSwordSwingEffect = LoadEffect("ArgaliaSwordSwing", true);
            argaliaEGOSwordSwingEffect = LoadEffect("ArgaliaEGOSwordSwing", true);
            argaliaSpearPierceEffect = LoadEffect("ArgaliaSpearPierce", true);
            argaliaEGOSpearPierceEffect = LoadEffect("ArgaliaEGOSpearPierce", true);
            argaliaHorizontalSwordSwingEffect = LoadEffect("ArgaliaHorizontalSwordSwing", true);
            argaliaSwordSpinEffect = LoadEffect("ArgaliaSpin", true);
            argaliaSwordSpinEffectTwo = LoadEffect("ArgaliaSpinTwo", true);
            argaliaSwordHitEffect = LoadEffect("ArgaliaSwordHitEffect");
            argaliaGroundPoundEffect = LoadEffect("ArgaliaGroundPound");
            argaliaCounterBurst = LoadEffect("ArgaliaCounterBurst");

            if (Config.redMistCoatShader.Value)
            {
                afterimageSlash = LoadEffect("RedMistAfterimageSwing", "Play_Kali_EGO_Hori");
                afterimageSlash.AddComponent<DestroyOnTimer>().duration = 0.75f;
                afterimageBlock = LoadEffect("RedMistAfterimageBlock", true);
                afterimageBlock.AddComponent<DestroyOnTimer>().duration = 0.5f;

                argaliaAfterimageSlash = LoadEffect("ArgaliaAfterimageSwing", "Play_Kali_EGO_Hori");
                argaliaAfterimageSlash.AddComponent<DestroyOnTimer>().duration = 0.75f;
                argaliaAfterimageBlock = LoadEffect("ArgaliaAfterimageBlock", true);
                argaliaAfterimageBlock.AddComponent<DestroyOnTimer>().duration = 0.5f;
            } else
            {
                afterimageSlash = LoadEffect("RedMistAfterimageSwingFallback", "Play_Kali_EGO_Hori");
                afterimageSlash.AddComponent<DestroyOnTimer>().duration = 0.75f;
                afterimageBlock = LoadEffect("RedMistAfterimageBlockFallback", true);
                afterimageBlock.AddComponent<DestroyOnTimer>().duration = 0.5f;

                argaliaAfterimageSlash = LoadEffect("ArgaliaAfterimageSwingFallback", "Play_Kali_EGO_Hori");
                argaliaAfterimageSlash.AddComponent<DestroyOnTimer>().duration = 0.75f;
                argaliaAfterimageBlock = LoadEffect("ArgaliaAfterimageBlockFallback", true);
                argaliaAfterimageBlock.AddComponent<DestroyOnTimer>().duration = 0.5f;
            }

                

            phaseEffect = LoadEffect("PhaseEffect"); 
            argaliaPhaseEffect = LoadEffect("ArgaliaPhaseEffect");
            phaseMistEffect = LoadEffect("PhaseMistEffect", true);

            EGOActivate = LoadEffect("TransformationBurst");
            argaliaEGOActivate = LoadEffect("ArgaliaTransformationBurst");
            EGODeactivate = LoadEffect("TransformationLost");

            swordHitSoundVert = CreateNetworkSoundEventDef("Play_Kali_Normal_Vert");
            swordHitSoundHori = CreateNetworkSoundEventDef("Play_Kali_Normal_Hori");
            swordHitSoundStab = CreateNetworkSoundEventDef("Play_Kali_Normal_Stab");
            swordHitEGOSoundVert = CreateNetworkSoundEventDef("Play_Kali_EGO_Vert");
            swordHitEGOSoundHori = CreateNetworkSoundEventDef("Play_Kali_EGO_Hori");
            swordHitEGOSoundStab = CreateNetworkSoundEventDef("Play_Kali_EGO_Stab");
            swordHitEGOSoundGRHorizontal = CreateNetworkSoundEventDef("Play_Kali_Special_Hori_Fin");


            // Arbiter Assets

            shockwaveEffect = mainAssetBundle.LoadAsset<GameObject>("ShockwaveEffect");

            armSwingEffect = Assets.LoadEffect("ArbiterArmSwingEffect", true);
            fairyProcEffect = LoadEffect("FairyProcEffect", "Play_Effect_Stun");
            if (Config.arbiterSound.Value)
            {
                fairyExplodeEffect = LoadEffect("FairyExplodeEffect", "Play_Binah_Fairy");
            } else
            {
                fairyExplodeEffect = LoadEffect("FairyExplodeEffect", "Play_Effect_Stun");
            }
            fairyHitEffect = LoadEffect("FairyHitEffect");
            fairyDeleteEffect = LoadEffect("FairyDeleteEffect", "Play_Effect_Stun");
            lockEffect5s = LoadEffect("LockEffect5s", "Play_Binah_Lock");
            lockEffect4s = LoadEffect("LockEffect4s", "Play_Binah_Lock");
            lockEffect3s = LoadEffect("LockEffect3s", "Play_Binah_Lock");
            lockEffect2s = LoadEffect("LockEffect2s", "Play_Binah_Lock");
            lockEffect1s = LoadEffect("LockEffect1s", "Play_Binah_Lock");
            lockEffectBreak = LoadEffect("LockEffectBreak", "Play_Binah_Lock");
            unlockEffect = LoadEffect("UnlockBurst", "Play_Binah_Chain");
            pillarImpactEffect = LoadEffect("PillarImpact");
            fairyTrail = mainAssetBundle.LoadAsset<GameObject>("FairyTrail");
            pillarSpear = mainAssetBundle.LoadAsset<GameObject>("PillarSpear");
            pillarObject = mainAssetBundle.LoadAsset<GameObject>("Pillar");

            fairyHitSound = CreateNetworkSoundEventDef("Play_Binah_Fairy");
        }

        private static GameObject CreateTracer(string originalTracerName, string newTracerName)
        {
            if (Resources.Load<GameObject>("Prefabs/Effects/Tracers/" + originalTracerName) == null) return null;

            GameObject newTracer = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Effects/Tracers/" + originalTracerName), newTracerName, true);

            if (!newTracer.GetComponent<EffectComponent>()) newTracer.AddComponent<EffectComponent>();
            if (!newTracer.GetComponent<VFXAttributes>()) newTracer.AddComponent<VFXAttributes>();
            if (!newTracer.GetComponent<NetworkIdentity>()) newTracer.AddComponent<NetworkIdentity>();

            newTracer.GetComponent<Tracer>().speed = 250f;
            newTracer.GetComponent<Tracer>().length = 50f;

            AddNewEffectDef(newTracer);

            return newTracer;
        }

        internal static NetworkSoundEventDef CreateNetworkSoundEventDef(string eventName)
        {
            NetworkSoundEventDef networkSoundEventDef = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            networkSoundEventDef.akId = AkSoundEngine.GetIDFromString(eventName);
            networkSoundEventDef.eventName = eventName;

            networkSoundEventDefs.Add(networkSoundEventDef);

            return networkSoundEventDef;
        }

        internal static void ConvertAllRenderersToHopooShader(GameObject objectToConvert)
        {
            if (!objectToConvert) return;

            foreach (MeshRenderer i in objectToConvert.GetComponentsInChildren<MeshRenderer>())
            {
                if (i)
                {
                    if (i.material)
                    {
                        i.material.shader = hotpoo;
                    }
                }
            }

            foreach (SkinnedMeshRenderer i in objectToConvert.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                if (i)
                {
                    if (i.material)
                    {
                        i.material.shader = hotpoo;
                    }
                }
            }
        }

        internal static CharacterModel.RendererInfo[] SetupRendererInfos(GameObject obj)
        {
            MeshRenderer[] meshes = obj.GetComponentsInChildren<MeshRenderer>();
            CharacterModel.RendererInfo[] rendererInfos = new CharacterModel.RendererInfo[meshes.Length];

            for (int i = 0; i < meshes.Length; i++)
            {
                rendererInfos[i] = new CharacterModel.RendererInfo
                {
                    defaultMaterial = meshes[i].material,
                    renderer = meshes[i],
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false
                };
            }

            return rendererInfos;
        }

        internal static Texture LoadCharacterIcon(string characterName)
        {
            return mainAssetBundle.LoadAsset<Texture>("tex" + characterName + "Icon");
        }

        internal static GameObject LoadCrosshair(string crosshairName)
        {
            if (Resources.Load<GameObject>("Prefabs/Crosshair/" + crosshairName + "Crosshair") == null) return Resources.Load<GameObject>("Prefabs/Crosshair/StandardCrosshair");
            return Resources.Load<GameObject>("Prefabs/Crosshair/" + crosshairName + "Crosshair");
        }

        private static GameObject LoadEffect(string resourceName)
        {
            return LoadEffect(resourceName, "", false);
        }

        private static GameObject LoadEffect(string resourceName, string soundName)
        {
            return LoadEffect(resourceName, soundName, false);
        }

        private static GameObject LoadEffect(string resourceName, bool parentToTransform)
        {
            return LoadEffect(resourceName, "", parentToTransform);
        }

        private static GameObject LoadEffect(string resourceName, string soundName, bool parentToTransform)
        {
            bool assetExists = false;
            for (int i = 0; i < assetNames.Length; i++)
            {
                if (assetNames[i].Contains(resourceName.ToLower()))
                {
                    assetExists = true;
                    i = assetNames.Length;
                }
            }

            if (!assetExists)
            {
                Debug.LogError("Failed to load effect: " + resourceName + " because it does not exist in the AssetBundle");
                return null;
            }

            GameObject newEffect = mainAssetBundle.LoadAsset<GameObject>(resourceName);

            newEffect.AddComponent<DestroyOnTimer>().duration = 12;
            newEffect.AddComponent<NetworkIdentity>();
            newEffect.AddComponent<VFXAttributes>().vfxPriority = VFXAttributes.VFXPriority.Always;
            var effect = newEffect.AddComponent<EffectComponent>();
            effect.applyScale = false;
            effect.effectIndex = EffectIndex.Invalid;
            effect.parentToReferencedTransform = parentToTransform;
            effect.positionAtReferencedTransform = true;
            effect.soundName = soundName;

            AddNewEffectDef(newEffect, soundName);

            return newEffect;
        }

        private static void AddNewEffectDef(GameObject effectPrefab)
        {
            AddNewEffectDef(effectPrefab, "");
        }

        private static void AddNewEffectDef(GameObject effectPrefab, string soundName)
        {
            EffectDef newEffectDef = new EffectDef();
            newEffectDef.prefab = effectPrefab;
            newEffectDef.prefabEffectComponent = effectPrefab.GetComponent<EffectComponent>();
            newEffectDef.prefabName = effectPrefab.name;
            newEffectDef.prefabVfxAttributes = effectPrefab.GetComponent<VFXAttributes>();
            newEffectDef.spawnSoundEventName = soundName;

            effectDefs.Add(newEffectDef);
        }

        public static Material CreateMaterial(string materialName, float emission, Color emissionColor, float normalStrength)
        {
            if (!commandoMat) commandoMat = Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").GetComponentInChildren<CharacterModel>().baseRendererInfos[0].defaultMaterial;

            Material mat = UnityEngine.Object.Instantiate<Material>(commandoMat);
            Material tempMat = Assets.mainAssetBundle.LoadAsset<Material>(materialName);

            if (!tempMat)
            {
                Debug.LogError("Failed to load material: " + materialName + " - Check to see that the name in your Unity project matches the one in this code");
                return commandoMat;
            }

            mat.name = materialName;
            mat.SetColor("_Color", tempMat.GetColor("_Color"));
            mat.SetTexture("_MainTex", tempMat.GetTexture("_MainTex"));
            mat.SetColor("_EmColor", emissionColor);
            mat.SetFloat("_EmPower", emission);
            mat.SetTexture("_EmTex", tempMat.GetTexture("_EmissionMap"));
            mat.SetFloat("_NormalStrength", normalStrength);

            return mat;
        }

        public static Material CreateMaterial(string materialName)
        {
            return Assets.CreateMaterial(materialName, 0f);
        }

        public static Material CreateMaterial(string materialName, float emission)
        {
            return Assets.CreateMaterial(materialName, emission, Color.white);
        }

        public static Material CreateMaterial(string materialName, float emission, Color emissionColor)
        {
            return Assets.CreateMaterial(materialName, emission, emissionColor, 0f);
        }
    }
}