using EntityStates;
using RoR2;
using RoR2.Projectile;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskOfRuinaMod.SkillStates.BaseStates
{
    public abstract class BaseCastChanneledSpellState : BaseSkillState
    {
        public Queue<GameObject> projectilePrefabs = new Queue<GameObject>();
        public GameObject muzzleflashEffectPrefab;
        public float baseDuration;
        public Vector3 spellPosition;
        public Quaternion spellRotation;
        public string castSoundString;
        public string muzzleString = "SpellCastEffect";
        public float baseInterval;
        public bool centered = false;

        public GenericSkill chainActivatorSkillSlot;

        protected float overrideDuration;

        private float duration;
        private float interval;
        public float charge;
        private float stopwatch = 0f;
        private float prevAge = 0f;
        private bool valid = true;

        public override void OnEnter()
        {
            if (this.spellPosition == Vector3.zero && this.spellRotation == Quaternion.identity)
            {
                chainActivatorSkillSlot.AddOneStock();
                this.outer.SetNextStateToMain();
                valid = false;
                return;
            }

            base.OnEnter();

            if (this.overrideDuration == 0) this.duration = this.baseDuration / (this.attackSpeedStat / 2f);
            else this.duration = this.overrideDuration;
            this.interval = this.baseInterval / (this.attackSpeedStat / 2f);

            this.PlayCastAnimation();

            if (this.muzzleflashEffectPrefab)
            {
                EffectManager.SimpleMuzzleFlash(this.muzzleflashEffectPrefab, base.gameObject, this.muzzleString, false);
            }

            if (NetworkServer.active) base.characterBody.AddBuff(RoR2Content.Buffs.Slow50);

            if (base.cameraTargetParams)
            {
                base.cameraTargetParams.aimMode = CameraTargetParams.AimType.Aura;
            }

            if (this.muzzleString == "SpellCastEffect")
            {
                ChildLocator childLocator = base.GetModelChildLocator();
                if (childLocator)
                {
                    GameObject castEffect = childLocator.FindChild("SpellCastEffect").gameObject;
                    castEffect.SetActive(false);
                    castEffect.SetActive(true);
                }
            }

            this.Fire();
        }

        protected virtual void PlayCastAnimation()
        {
            base.PlayAnimation("Gesture, Override", "CastSpell", "Spell.playbackRate", this.duration);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            base.characterBody.outOfCombatStopwatch = 0f;
            stopwatch += base.fixedAge - prevAge;
            prevAge = base.fixedAge;

            if (stopwatch >= (interval))
            {
                this.Fire();
                stopwatch = 0f;
            }

            if (base.isAuthority && base.fixedAge >= this.duration)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            if (NetworkServer.active) base.characterBody.RemoveBuff(RoR2Content.Buffs.Slow50);

            if (base.cameraTargetParams)
            {
                base.cameraTargetParams.aimMode = CameraTargetParams.AimType.Standard;
                base.cameraTargetParams.cameraParams = Modules.CameraParams.defaultCameraParamsArbiter;
            }
        }

        protected virtual void Fire()
        {
            if (projectilePrefabs.Count <= 0) return;

            if (this.castSoundString != "" && valid) Util.PlaySound(this.castSoundString, base.gameObject);

            GameObject projectilePrefab = projectilePrefabs.Dequeue();

            if (!projectilePrefab) return;

            if (base.isAuthority)
            {
                Ray aimRay = base.GetAimRay();

                if (projectilePrefab != null)
                {
                    Vector3 spawnPos = this.spellPosition;
                    Quaternion spawnRot = this.spellRotation;
                    if (centered)
                    {
                        spawnPos = base.transform.position;
                        spawnRot = Quaternion.identity;
                    }

                    FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
                    {
                        projectilePrefab = projectilePrefab,
                        position = spawnPos,
                        rotation = spawnRot,
                        owner = base.gameObject,
                        damage = this.damageStat,
                        force = 0f,
                        crit = base.RollCrit()
                    };

                    ProjectileManager.instance.FireProjectile(fireProjectileInfo);
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
