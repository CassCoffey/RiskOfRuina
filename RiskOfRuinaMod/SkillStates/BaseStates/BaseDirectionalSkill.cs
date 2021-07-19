using EntityStates;
using RiskOfRuinaMod.Modules.Components;
using RiskOfRuinaMod.Modules;
using RoR2;
using RoR2.Audio;
using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.CompilerServices;
using RiskOfRuinaMod.Modules.Survivors;

namespace RiskOfRuinaMod.SkillStates.BaseStates
{
    public class BaseDirectionalSkill : BaseSkillState
    {
        protected string hitboxName = "Sword";

        protected DamageType damageType = DamageType.Generic;
        protected float damageCoefficient = 1f;
        protected float procCoefficient = 1f;
        protected float pushForce = 300f;
        protected Vector3 bonusForce = Vector3.zero;
        protected float baseDuration = 1f;
        protected float attackStartTime = 0.2f;
        protected float attackEndTime = 0.4f;
        protected float baseEarlyExitTime = 0.4f;
        protected float hitStopDuration = 0.05f;
        protected float attackRecoil = 0f;
        protected float swingHopVelocity = 0f;
        protected bool cancelled = false;
        public int attackIndex = 1;

        protected string swingSoundString = "";
        protected string hitSoundString = "";
        protected string muzzleString = "SwingCenter";
        protected string attackAnimation = "Swing";
        protected GameObject swingEffectPrefab;
        protected GameObject hitEffectPrefab;
        protected NetworkSoundEventIndex impactSound;

        protected TemporaryOverlay iframeOverlay;

        private float earlyExitTime;
        public float duration;
        protected bool hasFired;
        private float hitPauseTimer;
        private OverlapAttack attack;
        protected bool inHitPause;
        private bool hasHopped;
        protected float stopwatch;
        protected Animator animator;
        private BaseState.HitStopCachedState hitStopCachedState;
        private Vector3 storedVelocity;
        public Vector2 inputVector;
        protected bool inAir;
        protected RedMistEmotionComponent emotionComponent;
        protected RedMistStatTracker statTracker;

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
            base.OnEnter();
            this.emotionComponent = base.gameObject.GetComponent<RedMistEmotionComponent>();
            this.statTracker = base.gameObject.GetComponent<RedMistStatTracker>();
            this.duration = this.baseDuration / trueAttackSpeed;
            this.earlyExitTime = this.baseEarlyExitTime / trueAttackSpeed;
            this.hasFired = false;
            this.animator = base.GetModelAnimator();
            base.StartAimMode(0.5f + this.duration, false);
            base.characterBody.outOfCombatStopwatch = 0f;
            this.animator.SetBool("attacking", true);

            HitBoxGroup hitBoxGroup = null;
            Transform modelTransform = base.GetModelTransform();

            if (modelTransform)
            {
                hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == this.hitboxName);
            }

            if (RiskOfRuinaPlugin.ancientScepterInstalled)
            {
                AncientScepterSetup();
            }

            this.PlayAttackAnimation();

            this.attack = new OverlapAttack();
            this.attack.damageType = this.damageType;
            this.attack.attacker = base.gameObject;
            this.attack.inflictor = base.gameObject;
            this.attack.teamIndex = base.GetTeam();
            this.attack.damage = this.damageCoefficient * this.trueDamage;
            this.attack.procCoefficient = this.procCoefficient;
            this.attack.hitEffectPrefab = this.hitEffectPrefab;
            this.attack.forceVector = this.bonusForce;
            this.attack.pushAwayForce = this.pushForce;
            this.attack.hitBoxGroup = hitBoxGroup;
            this.attack.isCrit = base.RollCrit();
            this.attack.impactSound = this.impactSound;
        }

        protected virtual void PlayAttackAnimation()
        {
            base.PlayCrossfade("Gesture, Override", attackAnimation, "BaseAttack.playbackRate", this.duration, 0.05f);
        }

        protected virtual void EvaluateInput()
        {
            Vector3 moveVector = base.inputBank.moveVector;
            Vector3 aimVector = base.inputBank.aimDirection;
            Vector3 forward = (new Vector3(aimVector.x, 0.0f, aimVector.z)).normalized;
            Vector3 up = base.transform.up;
            Vector3 right = Vector3.Cross(up, forward).normalized;
            inputVector = new Vector2(Vector3.Dot(moveVector, forward), Vector3.Dot(moveVector, right));
            inAir = !base.characterMotor.isGrounded;
        }

        public override void OnExit()
        {
            base.OnExit();

            this.animator.SetBool("attacking", false);
        }

        protected virtual void PlaySwingEffect()
        {
            EffectManager.SimpleMuzzleFlash(this.swingEffectPrefab, base.gameObject, this.muzzleString, true);
        }

        protected virtual void OnHitEnemyAuthority()
        {
            if (!this.inHitPause && this.hitStopDuration > 0f)
            {
                this.storedVelocity = base.characterMotor.velocity;
                this.hitStopCachedState = base.CreateHitStopCachedState(base.characterMotor, this.animator, "BaseAttack.playbackRate");
                this.hitPauseTimer = this.hitStopDuration / trueAttackSpeed;
                this.inHitPause = true;
            }
        }

        protected virtual void FireAttack()
        {
            if (!this.hasFired)
            {
                this.hasFired = true;
                Util.PlayAttackSpeedSound(this.swingSoundString, base.gameObject, trueAttackSpeed);

                if (base.isAuthority)
                {
                    this.PlaySwingEffect();
                    base.AddRecoil(-1f * this.attackRecoil, -2f * this.attackRecoil, -0.5f * this.attackRecoil, 0.5f * this.attackRecoil);
                }
            }

            if (base.isAuthority)
            {
                if (!this.hasHopped)
                {
                    if (base.characterMotor && !base.characterMotor.isGrounded && this.swingHopVelocity > 0f)
                    {
                        base.SmallHop(base.characterMotor, this.swingHopVelocity);
                    }

                    this.hasHopped = true;
                }

                if (this.attack.Fire())
                {
                    this.OnHitEnemyAuthority();
                }
            }
        }

        public override void FixedUpdate()
        {
            //Various cancels
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
                                this.storedVelocity = base.characterMotor.velocity;
                                this.hitStopCachedState = base.CreateHitStopCachedState(base.characterMotor, this.animator, "BaseAttack.playbackRate");
                                this.hitPauseTimer = 0.35f;
                                this.inHitPause = true;
                            }
                            else if(base.skillLocator.utility.baseSkill == RedMist.NormalDodge)
                            {
                                slideStateMachine.SetNextState(new EGODodge());
                                this.storedVelocity = base.characterMotor.velocity;
                                this.hitStopCachedState = base.CreateHitStopCachedState(base.characterMotor, this.animator, "BaseAttack.playbackRate");
                                this.hitPauseTimer = 0.3f;
                                this.inHitPause = true;
                            }
                        }
                    }
                } else
                {
                    if (base.skillLocator.utility.baseSkill == RedMist.NormalBlock)
                    {
                        this.outer.SetNextState(new Block());
                        return;
                    } else if (base.skillLocator.utility.baseSkill == RedMist.NormalDodge)
                    {
                        this.outer.SetNextState(new Dodge());
                        return;
                    }
                }
            } 
            else if (emotionComponent.inEGO && base.skillLocator.special.stock > 0 && base.inputBank.skill4.down)
            {
                base.skillLocator.special.stock--;
                this.outer.SetNextState(new EGOHorizontal
                {
                    attackIndex = 1,
                    inputVector = inputVector
                });
            } else if (!emotionComponent.inEGO && base.skillLocator.special.CanExecute() && base.skillLocator.special.stock > 0 && base.inputBank.skill4.down)
            {
                this.outer.SetNextState(new EGOActivate());
            }
            else if (base.skillLocator.secondary.stock > 0 && base.skillLocator.secondary.CanExecute() && base.inputBank.skill2.down)
            {
                base.skillLocator.secondary.stock--;
                this.outer.SetNextState(new Onrush
                {
                    chained = false
                });
            }

            base.FixedUpdate();

            EvaluateInput();

            this.hitPauseTimer -= Time.fixedDeltaTime;

            if (this.hitPauseTimer <= 0f && this.inHitPause)
            {
                base.ConsumeHitStopCachedState(this.hitStopCachedState, base.characterMotor, this.animator);
                this.inHitPause = false;
                base.characterMotor.velocity = this.storedVelocity;
            }

            if (!this.inHitPause)
            {
                this.stopwatch += Time.fixedDeltaTime;
            }
            else
            {
                if (base.characterMotor) base.characterMotor.velocity = Vector3.zero;
                if (this.animator) this.animator.SetFloat("BaseAttack.playbackRate", 0f);
            }

            if (this.stopwatch >= (this.duration * this.attackStartTime) && this.stopwatch <= (this.duration * this.attackEndTime))
            {
                this.FireAttack();
            }

            if (this.stopwatch >= (this.duration - this.earlyExitTime) && base.isAuthority && base.inputBank)
            {
                if (base.inputBank.skill1.down || base.inputBank.jump.down || inputVector != Vector2.zero || (emotionComponent.inEGO && base.skillLocator.special.stock > 0 && base.inputBank.skill4.down))
                {
                    if (!this.hasFired) this.FireAttack();
                    this.SetNextState();
                    return;
                }
            }

            if (this.stopwatch >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        protected virtual void SetNextState()
        {
            if (this.inputBank.skill1.down)
            {
                if (inAir)
                {
                    // Back Attack
                    if (inputVector.x < -0.5f)
                    {
                        if (emotionComponent.inEGO)
                        {
                            this.outer.SetNextState(new EGOAirBackAttack
                            {
                                attackIndex = 1,
                                inputVector = inputVector
                            });
                        } else
                        {
                            this.outer.SetNextState(new AirBackAttack
                            {
                                attackIndex = 1,
                                inputVector = inputVector
                            });
                        }
                    }
                    // Basic Attack
                    else
                    {
                        if (emotionComponent.inEGO)
                        {
                            if (this.GetType() == typeof(EGOAirBasicAttack))
                            {
                                this.outer.SetNextState(new EGOAirBasicAttack
                                {
                                    attackIndex = attackIndex + 1,
                                    inputVector = inputVector
                                });
                            }
                            else
                            {
                                this.outer.SetNextState(new EGOAirBasicAttack
                                {
                                    attackIndex = 1,
                                    inputVector = inputVector
                                });
                            }
                        } else
                        {
                            if (this.GetType() == typeof(AirBasicAttack))
                            {
                                this.outer.SetNextState(new AirBasicAttack
                                {
                                    attackIndex = attackIndex + 1,
                                    inputVector = inputVector
                                });
                            }
                            else
                            {
                                this.outer.SetNextState(new AirBasicAttack
                                {
                                    attackIndex = 1,
                                    inputVector = inputVector
                                });
                            }
                        }
                    }
                }
                else
                {
                    // Jump Attack
                    if (inputBank.jump.down)
                    {
                        if (emotionComponent.inEGO)
                        {
                            this.outer.SetNextState(new EGOJumpAttack
                            {
                                attackIndex = 1,
                                inputVector = inputVector
                            });
                        } else
                        {
                            this.outer.SetNextState(new JumpAttack
                            {
                                attackIndex = 1,
                                inputVector = inputVector
                            });
                        }
                    }
                    // Forward Attack
                    else if (inputVector.x > 0.5f)
                    {
                        if (emotionComponent.inEGO)
                        {
                            if (this.GetType() == typeof(EGOForwardAttack))
                            {
                                this.outer.SetNextState(new EGOForwardAttack
                                {
                                    attackIndex = attackIndex + 1,
                                    inputVector = inputVector
                                });
                            }
                            else
                            {
                                this.outer.SetNextState(new EGOForwardAttack
                                {
                                    attackIndex = 1,
                                    inputVector = inputVector
                                });
                            }
                        } else
                        {
                            if (this.GetType() == typeof(ForwardAttack))
                            {
                                this.outer.SetNextState(new ForwardAttack
                                {
                                    attackIndex = attackIndex + 1,
                                    inputVector = inputVector
                                });
                            }
                            else
                            {
                                this.outer.SetNextState(new ForwardAttack
                                {
                                    attackIndex = 1,
                                    inputVector = inputVector
                                });
                            }
                        } 
                    }
                    // Back Attack
                    else if (inputVector.x < -0.5f)
                    {
                        if (emotionComponent.inEGO)
                        {
                            this.outer.SetNextState(new EGOBackAttack
                            {
                                attackIndex = 1,
                                inputVector = inputVector
                            });
                        } else
                        {
                            this.outer.SetNextState(new BackAttack
                            {
                                attackIndex = 1,
                                inputVector = inputVector
                            });
                        }
                    }
                    // Left/Right Attack
                    else if (inputVector.y > 0.5f || inputVector.y < -0.5f)
                    {
                        if (emotionComponent.inEGO)
                        {
                            if (this.GetType() == typeof(EGOSideAttack))
                            {
                                this.outer.SetNextState(new EGOSideAttack
                                {
                                    attackIndex = attackIndex + 1,
                                    inputVector = inputVector
                                });
                            }
                            else
                            {
                                this.outer.SetNextState(new EGOSideAttack
                                {
                                    attackIndex = 1,
                                    inputVector = inputVector
                                });
                            }
                        } else
                        {
                            if (this.GetType() == typeof(SideAttack))
                            {
                                this.outer.SetNextState(new SideAttack
                                {
                                    attackIndex = attackIndex + 1,
                                    inputVector = inputVector
                                });
                            }
                            else
                            {
                                this.outer.SetNextState(new SideAttack
                                {
                                    attackIndex = 1,
                                    inputVector = inputVector
                                });
                            }
                        }
                            
                    }
                    // Basic Attack
                    else
                    {
                        if (emotionComponent.inEGO)
                        {
                            if (this.GetType() == typeof(EGOBasicAttack))
                            {
                                this.outer.SetNextState(new EGOBasicAttack
                                {
                                    attackIndex = attackIndex + 1,
                                    inputVector = inputVector
                                });
                            }
                            else
                            {
                                this.outer.SetNextState(new EGOBasicAttack
                                {
                                    attackIndex = 1,
                                    inputVector = inputVector
                                });
                            }
                        } else
                        {
                            if (this.GetType() == typeof(BasicAttack))
                            {
                                this.outer.SetNextState(new BasicAttack
                                {
                                    attackIndex = attackIndex + 1,
                                    inputVector = inputVector
                                });
                            }
                            else
                            {
                                this.outer.SetNextState(new BasicAttack
                                {
                                    attackIndex = 1,
                                    inputVector = inputVector
                                });
                            }
                        }
                    }
                }
            }
            else if (inputBank.jump.down)
            {
                this.outer.SetNextStateToMain();
            }
            else if (inputVector != Vector2.zero)
            {
                // clear the current move
                base.PlayAnimation("FullBody, Override", "BufferEmpty");
                this.animator.SetBool("isMoving", true);
                this.outer.SetNextStateToMain();
            }
        }

        protected void AddOverlay(float duration)
        {
            if (Config.iframeOverlay.Value)
            {
                iframeOverlay = base.characterBody.gameObject.AddComponent<TemporaryOverlay>();
                iframeOverlay.duration = duration;
                iframeOverlay.alphaCurve = AnimationCurve.Constant(0f, duration, 0.1f);
                iframeOverlay.animateShaderAlpha = true;
                iframeOverlay.destroyComponentOnEnd = true;
                iframeOverlay.originalMaterial = Resources.Load<Material>("Materials/matHuntressFlashBright");
                iframeOverlay.AddToCharacerModel(base.modelLocator.modelTransform.GetComponent<CharacterModel>());
            }
        }

        protected void RemoveOverlay()
        {
            if (iframeOverlay)
            {
                UnityEngine.Object.Destroy(iframeOverlay);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void AncientScepterSetup()
        {
            if (base.characterBody.inventory.GetItemCount(AncientScepter.AncientScepterItem.instance.ItemDef) > 0)
            {
                this.damageType = DamageType.BonusToLowHealth;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(this.inputVector);
            writer.Write(this.attackIndex);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            this.inputVector = reader.ReadVector2();
            this.attackIndex = reader.ReadInt32();
        }
    }
}
