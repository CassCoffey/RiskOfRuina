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
    public class Lock : BaseSkillState
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

        public override void OnEnter()
        {
            base.OnEnter();
            this.tracker = base.GetComponent<TargetTracker>();
            this.target = this.tracker.GetTrackingTarget();

            if (this.target && this.target.healthComponent && this.target.healthComponent.alive)
            {
                this.targetIsValid = true;
                Util.PlaySound("Play_Binah_Lock_Ready", base.gameObject);
            }
            else
            {
                base.activatorSkillSlot.AddOneStock();
                this.outer.SetNextStateToMain();
            }
            this.duration = Lock.baseDuration / this.attackSpeedStat;
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
                    int stacks = target.GetBuffCount(Modules.Buffs.lockResistBuff);
                    
                    if (NetworkServer.active)
                    {
                        DamageInfo lockDamage = new DamageInfo()
                        {
                            attacker = base.characterBody.gameObject,
                            inflictor = base.characterBody.gameObject,
                            crit = base.RollCrit(),
                            damage = base.characterBody.damage * Modules.StaticValues.lockDamageCoefficient,
                            position = this.target.transform.position,
                            force = UnityEngine.Vector3.zero,
                            damageType = RoR2.DamageType.Stun1s,
                            damageColorIndex = RoR2.DamageColorIndex.Default,
                            procCoefficient = 1f
                        };

                        target.healthComponent.TakeDamage(lockDamage);

                        GlobalEventManager.instance.OnHitEnemy(lockDamage, target.gameObject);
                        GlobalEventManager.instance.OnHitAll(lockDamage, target.gameObject);
                    }

                    if (stacks <= 4 && target.GetBuffCount(Modules.Buffs.lockDebuff) == 0)
                    {
                        if (NetworkServer.active)
                        {
                            target.AddTimedBuff(Modules.Buffs.lockDebuff, 5f - (float)stacks, 1);
                            target.AddBuff(Modules.Buffs.lockResistBuff);
                        }

                        Transform modelTransform = target.modelLocator.modelTransform;

                        if (target && modelTransform)
                        {
                            TemporaryOverlay overlay = target.gameObject.AddComponent<TemporaryOverlay>();
                            overlay.duration = 5f - (float)stacks;
                            overlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                            overlay.animateShaderAlpha = true;
                            overlay.destroyComponentOnEnd = true;
                            overlay.originalMaterial = Assets.mainAssetBundle.LoadAsset<Material>("matChains");
                            overlay.AddToCharacerModel(modelTransform.GetComponent<CharacterModel>());
                        }

                        EntityStateMachine component = target.GetComponent<EntityStateMachine>();

                        if (component != null)
                        {
                            LockState state = new LockState
                            {
                                duration = 5f - (float)stacks
                            };
                            component.SetState(state);
                        }
                    }

                    if (base.isAuthority)
                    {
                        int duration = 5 - stacks;

                        GameObject effect = null;

                        switch(duration)
                        {
                            case 5:
                                effect = Assets.lockEffect5s;
                                break;
                            case 4:
                                effect = Assets.lockEffect4s;
                                break;
                            case 3:
                                effect = Assets.lockEffect3s;
                                break;
                            case 2:
                                effect = Assets.lockEffect2s;
                                break;
                            case 1:
                                effect = Assets.lockEffect1s;
                                break;
                            default:
                                effect = Assets.lockEffectBreak;
                                break;
                        }

                        if (target.healthComponent && target.healthComponent.combinedHealthFraction <= 0)
                        {
                            // killed them, don't linger
                            effect = Assets.lockEffectBreak;
                        }

                        EffectData effectData = new EffectData();
                        effectData.rotation = Util.QuaternionSafeLookRotation(Vector3.zero);
                        effectData.origin = target.corePosition;

                        EffectManager.SpawnEffect(effect, effectData, true);
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