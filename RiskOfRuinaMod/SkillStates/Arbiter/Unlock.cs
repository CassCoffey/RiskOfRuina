using EntityStates;
using RoR2;
using UnityEngine;
using RiskOfRuinaMod.Modules.Components;
using UnityEngine.Networking;
using RiskOfRuinaMod.Modules.Misc;
using RoR2.Projectile;
using RiskOfRuinaMod.Modules;

namespace RiskOfRuinaMod.SkillStates
{
    public class Unlock : BaseSkillState
    {
        public static float procCoefficient = 1f;
        public static float baseDuration = 0.6f;
        public static float force = 800f;
        public static float recoil = 3f;
        public static float range = 256f;

        private float duration;
        private float fireTime;
        private bool hasFired;
        private string muzzleString;
        private TargetTracker tracker;
        private CharacterBody target;
        private bool targetIsValid;

        private int stocks = 0;

        public override void OnEnter()
        {
            base.OnEnter();
            
            this.tracker = base.GetComponent<TargetTracker>();
            this.target = this.tracker.GetTrackingTarget();

            stocks = base.activatorSkillSlot.stock;
            base.activatorSkillSlot.stock = 0;

            if (this.target && this.target.healthComponent && this.target.healthComponent.alive)
            {
                this.targetIsValid = true;
                Util.PlaySound("Play_Binah_Lock_Ready", base.gameObject);
            }
            else
            {
                base.activatorSkillSlot.stock = stocks;
                this.outer.SetNextStateToMain();
            }
            this.duration = Unlock.baseDuration / this.attackSpeedStat;
            this.fireTime = 0.2f * this.duration;
            base.characterBody.SetAimTimer(2f);
            this.muzzleString = "HandR";
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        private void Fire()
        {
            if (!this.hasFired)
            {
                this.hasFired = true;

                base.characterBody.AddSpreadBloom(1.5f);
                EffectManager.SimpleMuzzleFlash(EntityStates.Commando.CommandoWeapon.FirePistol2.muzzleEffectPrefab, base.gameObject, this.muzzleString, false);

                if (this.targetIsValid)
                {
                    if (NetworkServer.active)
                    {
                        target.AddTimedBuff(Buffs.strengthBuff, 10f * stocks);
                    }

                    Transform modelTransform = target.modelLocator.modelTransform;

                    if (target && modelTransform)
                    {
                        TemporaryOverlay overlay = target.gameObject.AddComponent<TemporaryOverlay>();
                        overlay.duration = 10f * stocks;
                        overlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                        overlay.animateShaderAlpha = true;
                        overlay.destroyComponentOnEnd = true;
                        overlay.originalMaterial = Assets.mainAssetBundle.LoadAsset<Material>("matChains");
                        overlay.AddToCharacerModel(modelTransform.GetComponent<CharacterModel>());
                    }

                    if (base.isAuthority)
                    {
                        EffectData effectData = new EffectData();
                        effectData.rotation = Util.QuaternionSafeLookRotation(Vector3.zero);
                        effectData.origin = target.corePosition;

                        EffectManager.SpawnEffect(Assets.unlockEffect, effectData, true);
                    }
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= this.fireTime)
            {
                this.Fire();
            }

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}