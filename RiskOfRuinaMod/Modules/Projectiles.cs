using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskOfRuinaMod.Modules
{
    internal static class Projectiles
    {
        internal static GameObject shockwaveSmallPrefab;
        internal static GameObject shockwaveMediumPrefab;
        internal static GameObject shockwaveLargePrefab;
        internal static GameObject shockwaveScepterPrefab;
        internal static GameObject fairyLinePrefab;
        internal static GameObject pillarPrefab;
        internal static GameObject pillarSpearPrefab;

        internal static void RegisterProjectiles()
        {
            // only separating into separate methods for my sanity
            CreateShockwaves();
            CreateFairyLine();
            CreatePillar();
            CreatePillarSpear();

            AddProjectile(shockwaveSmallPrefab);
            AddProjectile(shockwaveMediumPrefab);
            AddProjectile(shockwaveLargePrefab);
            AddProjectile(shockwaveScepterPrefab);
            AddProjectile(fairyLinePrefab);
            AddProjectile(pillarPrefab);
            AddProjectile(pillarSpearPrefab);
        }

        internal static void AddProjectile(GameObject projectileToAdd)
        {
            Modules.Prefabs.projectilePrefabs.Add(projectileToAdd);
        }

        private static void CreatePillar()
        {
            pillarPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Projectiles/SporeGrenadeProjectileDotZone"), "ArbiterPillar", true);
            pillarPrefab.transform.localScale = Vector3.one;

            RiskOfRuinaPlugin.Destroy(pillarPrefab.GetComponent<ProjectileDotZone>());

            Misc.ArbiterPillarController pillarController = pillarPrefab.AddComponent<Misc.ArbiterPillarController>();
            
            pillarController.radius = StaticValues.pillarRadius;
            pillarController.freezeProjectiles = true;
            pillarController.buffDef = Buffs.warpBuff;
            pillarController.buffDuration = 3f;
            pillarController.expires = true;
            pillarController.animateRadius = false;
            pillarController.interval = 1f;
            pillarController.expireDuration = StaticValues.pillarDuration;

            RiskOfRuinaPlugin.Destroy(pillarPrefab.transform.GetChild(0).gameObject);
            GameObject pillarFX = Assets.pillarObject.InstantiateClone("PillarEffect", false);
            pillarFX.transform.parent = pillarPrefab.transform;
            pillarFX.transform.localPosition = Vector3.zero;

            pillarPrefab.AddComponent<DestroyOnTimer>().duration = StaticValues.pillarDuration + 1f;
        }

        private static void CreatePillarSpear()
        {
            pillarSpearPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Projectiles/MageLightningBombProjectile"), "ArbiterPillarSpear", true);
            pillarSpearPrefab.transform.localScale = Vector3.one;

            Misc.DetonateFairyOnImpact detonate = pillarSpearPrefab.AddComponent<Misc.DetonateFairyOnImpact>();
            detonate.radius = StaticValues.pillarSpearRadius;

            GameObject spearGhost = Assets.pillarSpear.InstantiateClone("pillarSpearGhost", false);
            spearGhost.AddComponent<ProjectileGhostController>();

            pillarSpearPrefab.transform.localScale *= 2f;

            pillarSpearPrefab.GetComponent<ProjectileController>().ghostPrefab = spearGhost;
            pillarSpearPrefab.GetComponent<ProjectileDamage>().damageType = DamageType.Stun1s;
            pillarSpearPrefab.GetComponent<ProjectileImpactExplosion>().impactEffect = Assets.pillarImpactEffect;
            pillarSpearPrefab.GetComponent<ProjectileImpactExplosion>().blastRadius = StaticValues.pillarSpearRadius;
            pillarSpearPrefab.GetComponent<Rigidbody>().useGravity = false;

            RiskOfRuinaPlugin.Destroy(pillarSpearPrefab.GetComponent<AntiGravityForce>());
            RiskOfRuinaPlugin.Destroy(pillarSpearPrefab.GetComponent<ProjectileProximityBeamController>());
        }

        private static void CreateFairyLine()
        {
            fairyLinePrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Projectiles/MageLightningBombProjectile"), "FairyLine", true);

            GameObject lineGhost = Assets.fairyTrail.InstantiateClone("FairyLineGhost", false);
            lineGhost.AddComponent<ProjectileGhostController>();

            fairyLinePrefab.GetComponent<ProjectileController>().ghostPrefab = lineGhost;
            fairyLinePrefab.GetComponent<ProjectileController>().shouldPlaySounds = false;
            fairyLinePrefab.GetComponent<ProjectileDamage>().damageType = DamageType.Generic;
            fairyLinePrefab.GetComponent<ProjectileImpactExplosion>().impactEffect = Assets.fairyExplodeEffect;
            fairyLinePrefab.GetComponent<ProjectileImpactExplosion>().blastRadius = 10f;
            fairyLinePrefab.GetComponent<Rigidbody>().useGravity = false;

            RiskOfRuinaPlugin.Destroy(fairyLinePrefab.GetComponent<AntiGravityForce>());
            RiskOfRuinaPlugin.Destroy(fairyLinePrefab.GetComponent<AkEvent>());
            RiskOfRuinaPlugin.Destroy(fairyLinePrefab.GetComponent<ProjectileProximityBeamController>());
        }

        private static void CreateShockwaves()
        {
            shockwaveSmallPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Projectiles/SporeGrenadeProjectileDotZone"), "ArbiterShockwave", true);
            shockwaveSmallPrefab.transform.localScale = Vector3.one;

            RiskOfRuinaPlugin.Destroy(shockwaveSmallPrefab.GetComponent<ProjectileDotZone>());

            shockwaveSmallPrefab.AddComponent<DestroyOnTimer>().duration = 5f;

            Misc.ArbiterShockwaveController shockwaveController = shockwaveSmallPrefab.AddComponent<Misc.ArbiterShockwaveController>();
            
            shockwaveController.radius = StaticValues.shockwaveMinRadius;
            shockwaveController.barrierAmount = StaticValues.shockwaveMinBarrier;
            shockwaveController.destroyProjectiles = true;
            shockwaveController.buffDef = Buffs.feebleDebuff;
            shockwaveController.buffDuration = 10f;

            RiskOfRuinaPlugin.Destroy(shockwaveSmallPrefab.transform.GetChild(0).gameObject);
            GameObject shockwaveFX = Assets.shockwaveEffect.InstantiateClone("ShockwaveEffect", false);
            shockwaveFX.transform.parent = shockwaveSmallPrefab.transform;
            shockwaveFX.transform.localPosition = Vector3.zero;

            shockwaveFX.transform.localScale = Vector3.one * StaticValues.shockwaveMinRadius * 2f;



            shockwaveMediumPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Projectiles/SporeGrenadeProjectileDotZone"), "ArbiterShockwave", true);
            shockwaveMediumPrefab.transform.localScale = Vector3.one;

            RiskOfRuinaPlugin.Destroy(shockwaveMediumPrefab.GetComponent<ProjectileDotZone>());

            shockwaveMediumPrefab.AddComponent<DestroyOnTimer>().duration = 5f;

            shockwaveController = shockwaveMediumPrefab.AddComponent<Misc.ArbiterShockwaveController>();
            
            shockwaveController.radius = StaticValues.shockwaveMinRadius + ((StaticValues.shockwaveMaxRadius - StaticValues.shockwaveMinRadius) / 2f);
            shockwaveController.barrierAmount = StaticValues.shockwaveMinBarrier + ((StaticValues.shockwaveMaxBarrier - StaticValues.shockwaveMinBarrier) / 2f);
            shockwaveController.destroyProjectiles = true;
            shockwaveController.buffDef = Buffs.feebleDebuff;
            shockwaveController.buffDuration = 10f;

            RiskOfRuinaPlugin.Destroy(shockwaveMediumPrefab.transform.GetChild(0).gameObject);
            shockwaveFX = Assets.shockwaveEffect.InstantiateClone("ShockwaveEffect", false);
            shockwaveFX.transform.parent = shockwaveMediumPrefab.transform;
            shockwaveFX.transform.localPosition = Vector3.zero;

            shockwaveFX.transform.localScale = Vector3.one * (StaticValues.shockwaveMinRadius + ((StaticValues.shockwaveMaxRadius - StaticValues.shockwaveMinRadius) / 2f)) * 2f;



            shockwaveLargePrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Projectiles/SporeGrenadeProjectileDotZone"), "ArbiterShockwave", true);
            shockwaveLargePrefab.transform.localScale = Vector3.one;

            RiskOfRuinaPlugin.Destroy(shockwaveLargePrefab.GetComponent<ProjectileDotZone>());

            shockwaveLargePrefab.AddComponent<DestroyOnTimer>().duration = 5f;

            shockwaveController = shockwaveLargePrefab.AddComponent<Misc.ArbiterShockwaveController>();
            
            shockwaveController.radius = StaticValues.shockwaveMaxRadius;
            shockwaveController.barrierAmount = StaticValues.shockwaveMaxBarrier;
            shockwaveController.destroyProjectiles = true;
            shockwaveController.buffDef = Buffs.feebleDebuff;
            shockwaveController.buffDuration = 10f;

            RiskOfRuinaPlugin.Destroy(shockwaveLargePrefab.transform.GetChild(0).gameObject);
            shockwaveFX = Assets.shockwaveEffect.InstantiateClone("ShockwaveEffect", false);
            shockwaveFX.transform.parent = shockwaveLargePrefab.transform;
            shockwaveFX.transform.localPosition = Vector3.zero;

            shockwaveFX.transform.localScale = Vector3.one * StaticValues.shockwaveMaxRadius * 2f;



            shockwaveScepterPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Projectiles/SporeGrenadeProjectileDotZone"), "ArbiterShockwave", true);
            shockwaveScepterPrefab.transform.localScale = Vector3.one;

            RiskOfRuinaPlugin.Destroy(shockwaveScepterPrefab.GetComponent<ProjectileDotZone>());

            shockwaveScepterPrefab.AddComponent<DestroyOnTimer>().duration = 5f;

            shockwaveController = shockwaveScepterPrefab.AddComponent<Misc.ArbiterShockwaveController>();
            
            shockwaveController.radius = StaticValues.shockwaveScepterRadius;
            shockwaveController.barrierAmount = StaticValues.shockwaveScepterBarrier;
            shockwaveController.destroyProjectiles = true;
            shockwaveController.buffDef = Buffs.feebleDebuff;
            shockwaveController.buffDuration = 15f;

            RiskOfRuinaPlugin.Destroy(shockwaveScepterPrefab.transform.GetChild(0).gameObject);
            shockwaveFX = Assets.shockwaveEffect.InstantiateClone("ShockwaveEffect", false);
            shockwaveFX.transform.parent = shockwaveScepterPrefab.transform;
            shockwaveFX.transform.localPosition = Vector3.zero;

            shockwaveFX.transform.localScale = Vector3.one * StaticValues.shockwaveScepterRadius * 2f;
        }

        private static void InitializeImpactExplosion(ProjectileImpactExplosion projectileImpactExplosion)
        {
            projectileImpactExplosion.blastDamageCoefficient = 1f;
            projectileImpactExplosion.blastProcCoefficient = 1f;
            projectileImpactExplosion.blastRadius = 1f;
            projectileImpactExplosion.bonusBlastForce = Vector3.zero;
            projectileImpactExplosion.childrenCount = 0;
            projectileImpactExplosion.childrenDamageCoefficient = 0f;
            projectileImpactExplosion.childrenProjectilePrefab = null;
            projectileImpactExplosion.destroyOnEnemy = false;
            projectileImpactExplosion.destroyOnWorld = false;
            projectileImpactExplosion.explosionSoundString = "";
            projectileImpactExplosion.falloffModel = RoR2.BlastAttack.FalloffModel.None;
            projectileImpactExplosion.fireChildren = false;
            projectileImpactExplosion.impactEffect = null;
            projectileImpactExplosion.lifetime = 0f;
            projectileImpactExplosion.lifetimeAfterImpact = 0f;
            projectileImpactExplosion.lifetimeExpiredSoundString = "";
            projectileImpactExplosion.lifetimeRandomOffset = 0f;
            projectileImpactExplosion.offsetForLifetimeExpiredSound = 0f;
            projectileImpactExplosion.timerAfterImpact = false;

            projectileImpactExplosion.GetComponent<ProjectileDamage>().damageType = DamageType.Generic;
        }

        private static GameObject CreateGhostPrefab(string ghostName)
        {
            GameObject ghostPrefab = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>(ghostName);
            if (!ghostPrefab.GetComponent<NetworkIdentity>()) ghostPrefab.AddComponent<NetworkIdentity>();
            if (!ghostPrefab.GetComponent<ProjectileGhostController>()) ghostPrefab.AddComponent<ProjectileGhostController>();

            Modules.Assets.ConvertAllRenderersToHopooShader(ghostPrefab);

            return ghostPrefab;
        }

        private static GameObject CloneProjectilePrefab(string prefabName, string newPrefabName)
        {
            GameObject newPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Projectiles/" + prefabName), newPrefabName);
            return newPrefab;
        }
    }
}