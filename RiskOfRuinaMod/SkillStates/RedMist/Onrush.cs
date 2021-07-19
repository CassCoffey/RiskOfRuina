using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using RoR2.Projectile;
using RiskOfRuinaMod.Modules.Components;
using EntityStates.Mage;
using System.Collections.Generic;
using System.Linq;
using RiskOfRuinaMod.Modules;
using RiskOfRuinaMod.Modules.Survivors;

namespace RiskOfRuinaMod.SkillStates
{
    public class Onrush : BaseSkillState
    {
        public bool chained = false;
        public bool autoAim = false;

        private float startTime;
        private bool hasFired;
        private TargetTracker tracker;
        private RedMistEmotionComponent emotionComponent;
        private CharacterBody target = null;
        private NetworkInstanceId targetID;
        private bool targetIsValid = false;
        protected bool inAir;

        private float lungeDistance;
        private float lungeDuration;
        private float lungeStartTime;

        private float cooldownStartTime;
        private float cooldownDuration;

        private bool firstDash = true;
        private bool lunging = false;
        private bool cooldown = false;
        private Vector3 lungeTarget = Vector3.zero;
        private Vector3 dirToLungeTarget = Vector3.zero;

        private ParticleSystem mistEffect;

        private bool dud = false;

        private float originalTurnSpeed;

        protected float trueMoveSpeed
        {
            get { return this.GetComponent<RedMistStatTracker>().modifiedMoveSpeed; }
        }

        protected float trueAttackSpeed
        {
            get { return this.GetComponent<RedMistStatTracker>().modifiedAttackSpeed; }
        }

        protected float trueDamage
        {
            get { return (1.0f + (Config.redMistBuffDamage.Value * (float)this.characterBody.GetBuffCount(Modules.Buffs.RedMistBuff))) * (this.damageStat + (this.GetComponent<RedMistStatTracker>().DifferenceAttackSpeed * Config.attackSpeedMult.Value) + (this.GetComponent<RedMistStatTracker>().DifferenceMoveSpeed * Config.moveSpeedMult.Value)); }
        }


        public override void OnEnter()
        {
            this.tracker = base.GetComponent<TargetTracker>();
            this.emotionComponent = base.GetComponent<RedMistEmotionComponent>();
            this.originalTurnSpeed = base.characterDirection.turnSpeed;
            base.characterDirection.turnSpeed = 0f;

            this.mistEffect = base.GetComponent<RedMistStatTracker>().mistEffect;

            if (autoAim)
            {
                List<HurtBox> candidates = new List<HurtBox>();
                SphereSearch search = new SphereSearch();
                search.mask = LayerIndex.entityPrecise.mask;
                search.radius = tracker.maxTrackingDistance;
                search.ClearCandidates();
                search.origin = this.characterBody.corePosition;
                search.RefreshCandidates();
                search.FilterCandidatesByDistinctHurtBoxEntities();
                TeamMask mask = TeamMask.GetEnemyTeams(base.teamComponent.teamIndex);
                mask.RemoveTeam(TeamIndex.Neutral);
                search.FilterCandidatesByHurtBoxTeam(mask);
                search.OrderCandidatesByDistance();
                search.GetHurtBoxes(candidates);
                List<HurtBox> SortedList = candidates.OrderBy(o => o.healthComponent.health).ToList();

                if (SortedList.Count > 0)
                {
                    foreach (HurtBox candidate in SortedList)
                    {
                        if (candidate.healthComponent && candidate.healthComponent.body)
                        {
                            // Check line of sight
                            if (!Physics.Linecast(this.characterBody.corePosition, candidate.healthComponent.body.corePosition, 1 << 11))
                            {
                                this.target = candidate.healthComponent.body;
                                break;
                            }
                        }
                    }

                    if (this.target == null)
                    {
                        dud = true;
                        if (base.skillLocator.secondary.stock < base.skillLocator.secondary.maxStock)
                        {
                            base.skillLocator.secondary.AddOneStock();
                        }
                        if (base.isAuthority) this.outer.SetNextStateToMain();
                        return;
                    }
                } else
                {
                    dud = true;
                    if (base.skillLocator.secondary.stock < base.skillLocator.secondary.maxStock)
                    {
                        base.skillLocator.secondary.AddOneStock();
                    }
                    if (base.isAuthority) this.outer.SetNextStateToMain();
                    return;
                }

            } else
            {
                if (this.tracker.GetTrackingTarget())
                {
                    this.target = this.tracker.GetTrackingTarget();
                }
            }

            lungeDistance = 10f;
            lungeDuration = 0.3f;
            startTime = 0.4f;
            cooldownDuration = 0.8f;

            if (chained)
            {
                startTime = 0.0f;
                firstDash = false;
                if (NetworkServer.active) base.characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
                if (emotionComponent.inEGO)
                {
                    base.PlayCrossfade("FullBody, Override", "EGOOnrushContinue", "Onrush.playbackRate", 20f, 0.1f);
                    this.mistEffect.Play();
                } else
                {
                    base.PlayCrossfade("FullBody, Override", "OnrushContinue", "Onrush.playbackRate", 20f, 0.1f);
                }
            }

            if (base.isAuthority)
            {
                TargetSetup();
            }

            base.OnEnter();
        }

        private void TargetSetup()
        {
            if (this.target && this.target.healthComponent && this.target.healthComponent.alive)
            {
                targetID = this.target.netId;
                this.targetIsValid = true;
                if (emotionComponent.inEGO)
                {
                    if (startTime != 0.0f) base.PlayCrossfade("FullBody, Override", "EGOOnrush", "Onrush.playbackRate", this.startTime, 0.1f);
                }
                else
                {
                    if (startTime != 0.0f) base.PlayCrossfade("FullBody, Override", "Onrush", "Onrush.playbackRate", this.startTime, 0.1f);
                }
            }
            else
            {
                dud = true;
                if (base.skillLocator.secondary.stock < base.skillLocator.secondary.maxStock)
                {
                    base.skillLocator.secondary.AddOneStock();
                }
                if (base.isAuthority) this.outer.SetNextStateToMain();
                return;
            }
        }

        public override void OnExit()
        {
            this.mistEffect.Stop();

            base.characterDirection.turnSpeed = originalTurnSpeed;

            if (NetworkServer.active && base.characterBody.HasBuff(RoR2Content.Buffs.HiddenInvincibility))
            {
                base.characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
            }

            base.OnExit();
        }

        private void Fire()
        {
            if (!this.hasFired && this.target && this.target.healthComponent)
            {
                this.hasFired = true;

                if (this.targetIsValid)
                {
                    if (this.target.healthComponent.body && base.isAuthority)
                    {
                        EffectData effectData = new EffectData();
                        effectData.origin = this.target.healthComponent.body.corePosition;
                        effectData.scale = 1;
                        effectData.rotation = Quaternion.LookRotation(dirToLungeTarget);
                        EffectManager.SpawnEffect(this.GetComponent<RedMistStatTracker>().slashPrefab, effectData, true);
                    }

                    if (NetworkServer.active)
                    {
                        float damage = trueDamage * Modules.StaticValues.onrushDamageCoefficient;

                        DamageInfo onrushDamage = new DamageInfo()
                        {
                            attacker = base.characterBody.gameObject,
                            inflictor = base.characterBody.gameObject,
                            crit = base.RollCrit(),
                            damage = damage,
                            position = this.target.transform.position,
                            force = UnityEngine.Vector3.zero,
                            damageType = RoR2.DamageType.Generic,
                            damageColorIndex = RoR2.DamageColorIndex.Default,
                            procCoefficient = 1f
                        };

                        target.healthComponent.TakeDamage(onrushDamage);

                        GlobalEventManager.instance.OnHitEnemy(onrushDamage, target.healthComponent.body.gameObject);
                        GlobalEventManager.instance.OnHitAll(onrushDamage, target.healthComponent.body.gameObject);
                    }
                }
            }
        }

        public override void FixedUpdate()
        {
            if (this.inputBank.skill3.down)
            {
                if (emotionComponent.inEGO)
                {
                    EntityStateMachine slideStateMachine = null;
                    foreach (EntityStateMachine i in base.gameObject.GetComponents<EntityStateMachine>())
                    {
                        if (i)
                        {
                            if (i.customName == "Slide")
                            {
                                slideStateMachine = i;
                            }
                        }
                    }
                    
                    if (slideStateMachine != null)
                    {
                        if (slideStateMachine.CanInterruptState(InterruptPriority.PrioritySkill))
                        {
                            if (base.skillLocator.utility.baseSkill == RedMist.NormalBlock)
                            {
                                slideStateMachine.SetNextState(new EGOBlock());
                                this.outer.SetNextStateToMain();
                            }
                            else if (base.skillLocator.utility.baseSkill == RedMist.NormalDodge)
                            {
                                slideStateMachine.SetNextState(new EGODodge());
                                this.outer.SetNextStateToMain();
                            }
                        }
                    }
                }
                else
                {
                    if (base.skillLocator.utility.baseSkill == RedMist.NormalBlock)
                    {
                        this.outer.SetNextState(new Block());
                        return;
                    }
                    else if (base.skillLocator.utility.baseSkill == RedMist.NormalDodge)
                    {
                        this.outer.SetNextState(new Dodge());
                        return;
                    }
                }
            }
            else if(emotionComponent.inEGO && base.skillLocator.special.stock > 0 && base.inputBank.skill4.down)
            {
                base.skillLocator.special.stock--;
                this.outer.SetNextState(new EGOHorizontal
                {
                    attackIndex = 1,
                    inputVector = Vector3.zero
                });
            }
            else if (!emotionComponent.inEGO && base.skillLocator.special.CanExecute() && base.skillLocator.special.stock > 0 && base.inputBank.skill4.down)
            {
                this.outer.SetNextState(new EGOActivate());
            }

            base.FixedUpdate();

            if (NetworkServer.active && !targetIsValid)
            {
                GameObject candidate = NetworkServer.FindLocalObject(targetID);
                if (candidate && candidate.GetComponent<CharacterBody>())
                {
                    target = candidate.GetComponent<CharacterBody>();
                    TargetSetup();
                }
            }

            if (targetIsValid)
            {
                if (hasFired)
                {
                    if (lunging)
                    {
                        Lunge();
                    }

                    if (cooldown)
                    {
                        Cooldown();
                    }
                }
                else
                {
                    if (this.target && this.target.healthComponent && this.target.healthComponent.alive)
                    {
                        if (base.fixedAge >= this.startTime)
                        {
                            Dash();
                        }
                    }
                    else
                    {
                        if (base.skillLocator.secondary.stock < base.skillLocator.secondary.maxStock)
                        {
                            base.skillLocator.secondary.AddOneStock();
                        }

                        if (base.fixedAge >= this.startTime && !dud)
                        {
                            hasFired = true;
                            lunging = false;
                            cooldown = true;
                            cooldownStartTime = base.fixedAge;
                            cooldownDuration = 0.05f;
                        } else
                        {
                            if (base.isAuthority) this.outer.SetNextStateToMain();
                        }
                    }
                }
            }
        }

        private void Dash()
        {
            if (firstDash)
            {
                if (NetworkServer.active) base.characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
                if (emotionComponent.inEGO)
                {
                    this.mistEffect.Play();
                    Util.PlaySound("Play_Effect_Index_Unlock_Short", base.gameObject);
                    base.PlayCrossfade("FullBody, Override", "EGOOnrushContinue", "Onrush.playbackRate", 20f, 0.1f);
                } else
                {
                    Util.PlaySound("Ruina_Swipe", base.gameObject);
                    base.PlayCrossfade("FullBody, Override", "OnrushContinue", "Onrush.playbackRate", 20f, 0.1f);
                }
                firstDash = false;
            }

            if (base.inputBank.jump.down && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }

            Vector3 modifiedTarget = target.healthComponent.body.corePosition + ((base.characterBody.corePosition - target.healthComponent.body.corePosition).normalized * lungeDistance);
            Vector3 dirToTarget = (modifiedTarget - base.characterBody.corePosition).normalized;

            float speed = 10f;
            if (emotionComponent.inEGO) speed = 12f;

            base.characterMotor.rootMotion += dirToTarget * trueMoveSpeed * speed * Time.fixedDeltaTime;
            base.characterMotor.velocity = Vector3.zero;
            base.characterDirection.forward = dirToTarget;

            float distanceToTarget = Vector3.Distance(base.characterBody.corePosition, target.healthComponent.body.corePosition);

            if (distanceToTarget <= lungeDistance)
            {
                Util.PlaySound("Play_Kali_Special_Cut", base.gameObject);
                if (emotionComponent.inEGO)
                {
                    base.PlayCrossfade("FullBody, Override", "EGOOnrushFinish", "Onrush.playbackRate", this.lungeDuration + this.cooldownDuration, 0.1f);
                }
                else
                {
                    base.PlayCrossfade("FullBody, Override", "OnrushFinish", "Onrush.playbackRate", this.lungeDuration + this.cooldownDuration, 0.1f);
                }
                lunging = true;
                lungeTarget = target.healthComponent.body.corePosition - ((base.characterBody.corePosition - target.healthComponent.body.corePosition).normalized * lungeDistance);
                dirToLungeTarget = (lungeTarget - base.characterBody.corePosition).normalized;
                lungeStartTime = base.fixedAge;
                Fire();
                return;
            }
        }

        private void Lunge()
        {
            if ((base.fixedAge - this.lungeStartTime) < this.lungeDuration)
            {
                base.characterMotor.rootMotion += dirToLungeTarget * (20f * FlyUpState.speedCoefficientCurve.Evaluate((base.fixedAge - this.lungeStartTime) / this.lungeDuration) * Time.fixedDeltaTime);
                base.characterMotor.velocity = Vector3.zero;
                base.characterDirection.forward = dirToLungeTarget;
            }
            else
            {
                if (this.targetIsValid)
                {
                    if (target && target.healthComponent)
                    {
                        // killed them, refund
                        if (target.healthComponent.combinedHealthFraction <= 0)
                        {
                            if (base.skillLocator.secondary.stock < base.skillLocator.secondary.maxStock)
                            {
                                base.skillLocator.secondary.AddOneStock();
                            }
                        }
                    }
                    else
                    {
                        // killed them, refund
                        if (base.skillLocator.secondary.stock < base.skillLocator.secondary.maxStock)
                        {
                            base.skillLocator.secondary.AddOneStock();
                        }
                    }
                }

                if (NetworkServer.active)
                {
                    if (base.characterBody.HasBuff(RoR2Content.Buffs.HiddenInvincibility))
                    {
                        base.characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
                    }
                }

                lunging = false;
                cooldown = true;
                cooldownStartTime = base.fixedAge;
            }
        }

        private void Cooldown()
        {
            if ((base.fixedAge - this.cooldownStartTime) < this.cooldownDuration)
            {
                this.mistEffect.Stop();
                base.transform.position = lungeTarget;
                base.characterMotor.velocity = Vector3.zero;
                base.characterDirection.forward = dirToLungeTarget;

                if (base.isAuthority && emotionComponent.inEGO && base.skillLocator.secondary.stock > 0)
                {
                    base.skillLocator.secondary.stock--;

                    this.outer.SetNextState(new Onrush
                    {
                        chained = true,
                        autoAim = true
                    });
                    return;
                }

                if (base.isAuthority && base.inputBank.jump.down || base.inputBank.skill1.down || base.inputBank.skill2.down)
                {
                    SetNextState();
                }
                return;
            }
            else
            {
                if (base.isAuthority) this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        protected virtual void SetNextState()
        {
            if (this.inputBank.skill2.down)
            {
                if (base.skillLocator.secondary.stock > 0)
                {
                    base.skillLocator.secondary.stock--;
                    this.outer.SetNextState(new Onrush
                    {
                        chained = true
                    });
                }
            }
            else if (this.inputBank.jump.down)
            {
                this.outer.SetNextStateToMain();
                base.SmallHop(base.characterMotor, 8f);
            }
            else
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(this.chained);
            writer.Write(this.autoAim);
            writer.Write(this.targetID);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            this.chained = reader.ReadBoolean();
            this.autoAim = reader.ReadBoolean();
            this.targetID = reader.ReadNetworkId();
        }
    }
}
