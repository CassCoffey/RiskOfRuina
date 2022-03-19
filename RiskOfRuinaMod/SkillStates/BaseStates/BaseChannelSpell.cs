using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskOfRuinaMod.SkillStates.BaseStates
{
    public abstract class BaseChannelSpellState : BaseSkillState
    {
        protected abstract BaseCastChanneledSpellState GetNextState();
        public GameObject chargeEffectPrefab;
        public string chargeSoundString;
        public string startChargeSoundString = "";
        public GameObject crosshairOverridePrefab;
        public float maxSpellRadius;
        public float baseDuration = 3f;
        public Material overrideAreaIndicatorMat;
        public bool zooming = true;
        public bool centered = false;
        public bool line = false;

        private bool hasCharged;
        private GameObject defaultCrosshairPrefab;
        private CharacterCameraParams defaultCameraParams;
        private uint loopSoundInstanceId;
        private float duration { get; set; }
        private Animator animator { get; set; }
        private ChildLocator childLocator { get; set; }
        private GameObject chargeEffectInstance { get; set; }
        protected GameObject areaIndicatorInstance { get; set; }
 
        private CameraTargetParams.AimRequest aimRequest;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = this.baseDuration / (this.attackSpeedStat / 2f);
            this.animator = base.GetModelAnimator();
            this.childLocator = base.GetModelChildLocator();

            if (this.childLocator)
            {
                Transform transform = this.childLocator.FindChild("HandL");

                if (transform && this.chargeEffectPrefab)
                {
                    this.chargeEffectInstance = UnityEngine.Object.Instantiate<GameObject>(this.chargeEffectPrefab, transform.position, transform.rotation);
                    this.chargeEffectInstance.transform.parent = transform;

                    ScaleParticleSystemDuration scaleParticleSystemDuration = this.chargeEffectInstance.GetComponent<ScaleParticleSystemDuration>();
                    ObjectScaleCurve scaleCurve = this.chargeEffectInstance.GetComponent<ObjectScaleCurve>();

                    if (scaleParticleSystemDuration) scaleParticleSystemDuration.newDuration = this.duration;
                    if (scaleCurve) scaleCurve.timeMax = this.duration;
                }
            }

            this.PlayChannelAnimation();
            if (this.startChargeSoundString != "") Util.PlaySound(this.startChargeSoundString, base.gameObject);
            this.loopSoundInstanceId = Util.PlayAttackSpeedSound(this.chargeSoundString, base.gameObject, this.attackSpeedStat);
            this.defaultCrosshairPrefab = base.characterBody._defaultCrosshairPrefab;

            if (this.crosshairOverridePrefab)
            {
                base.characterBody._defaultCrosshairPrefab = this.crosshairOverridePrefab;
            }

            if (NetworkServer.active) base.characterBody.AddBuff(RoR2Content.Buffs.Slow50);

            if (EntityStates.Huntress.ArrowRain.areaIndicatorPrefab)
            {
                this.areaIndicatorInstance = UnityEngine.Object.Instantiate<GameObject>(EntityStates.Huntress.ArrowRain.areaIndicatorPrefab);

                if (line)
                {
                    GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    Destroy(temp.GetComponent<CapsuleCollider>());
                    temp.transform.parent = this.areaIndicatorInstance.transform;
                    temp.transform.localPosition = new Vector3(0f, 0.35f, 0f);
                    temp.transform.localScale = new Vector3(0.15f, 0.4f, 0.15f);
                    temp.GetComponent<MeshRenderer>().material = this.areaIndicatorInstance.GetComponentInChildren<MeshRenderer>().material;
                }
                
                this.areaIndicatorInstance.transform.localScale = Vector3.zero;

                if (this.overrideAreaIndicatorMat) this.areaIndicatorInstance.GetComponentInChildren<MeshRenderer>().material = this.overrideAreaIndicatorMat;
            }

            if (this.zooming)
            {
                this.defaultCameraParams = base.cameraTargetParams.cameraParams;
                base.cameraTargetParams.cameraParams = Modules.CameraParams.channelCameraParamsArbiter;
            }
        }

        protected virtual void PlayChannelAnimation()
        {
            base.PlayAnimation("Gesture, Override", "ChannelSpell", "Spell.playbackRate", 0.85f);
        }

        private void UpdateAreaIndicator()
        {
            if (this.areaIndicatorInstance)
            {
                if (centered)
                {
                    this.areaIndicatorInstance.transform.position = base.transform.position;
                    this.areaIndicatorInstance.transform.up = Vector3.up;
                } else
                {
                    float maxDistance = 128f;

                    Ray aimRay = base.GetAimRay();
                    RaycastHit raycastHit;
                    int worldLayerMask = 1 << LayerIndex.world.intVal;
                    if (Physics.Raycast(aimRay, out raycastHit, maxDistance, worldLayerMask))
                    {
                        if (!this.areaIndicatorInstance.activeSelf) this.areaIndicatorInstance.SetActive(true);
                        this.areaIndicatorInstance.transform.position = raycastHit.point;
                        this.areaIndicatorInstance.transform.up = raycastHit.normal;
                    }
                    else
                    {
                        if (this.areaIndicatorInstance.activeSelf) this.areaIndicatorInstance.SetActive(false);
                        this.areaIndicatorInstance.transform.position = aimRay.GetPoint(maxDistance);
                        this.areaIndicatorInstance.transform.up = -aimRay.direction;
                    }
                }
            }
        }

        public override void OnExit()
        {
            if (this.crosshairOverridePrefab)
            {
                base.characterBody._defaultCrosshairPrefab = this.defaultCrosshairPrefab;
            }
            else
            {
                base.characterBody.hideCrosshair = false;
            }

            if (this.areaIndicatorInstance)
            {
                EntityState.Destroy(this.areaIndicatorInstance.gameObject);
            }

            AkSoundEngine.StopPlayingID(this.loopSoundInstanceId);

            if (!this.outer.destroying)
            {
                this.EndAnimation();
            }

            if (this.zooming)
            {
                base.cameraTargetParams.cameraParams = Modules.CameraParams.defaultCameraParamsArbiter;
                aimRequest?.Dispose();
            }
            if (NetworkServer.active) base.characterBody.RemoveBuff(RoR2Content.Buffs.Slow50);

            if (this.chargeEffectInstance) EntityState.Destroy(this.chargeEffectInstance);

            base.OnExit();
        }

        protected virtual void EndAnimation()
        {
            base.PlayAnimation("Gesture, Override", "BufferEmpty");
        }

        protected float CalcCharge()
        {
            return Mathf.Clamp01(base.fixedAge / this.duration);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            base.characterBody.isSprinting = false;
            base.StartAimMode(0.5f, false);
            base.characterBody.outOfCombatStopwatch = 0f;

            float charge = this.CalcCharge();

            if (this.areaIndicatorInstance)
            {
                float size = Util.Remap(charge, 0, 1, 0, this.maxSpellRadius);

                this.areaIndicatorInstance.transform.localScale = new Vector3(size, size, size);
            }

            if (charge >= 0.75f && this.zooming)
            {
                base.cameraTargetParams.cameraParams = Modules.CameraParams.channelFullCameraParamsArbiter;
                aimRequest = base.cameraTargetParams.RequestAimType(CameraTargetParams.AimType.Aura);
            }

            if (charge >= 1f)
            {
                if (!this.hasCharged)
                {
                    this.hasCharged = true;
                }
            }

            if (base.isAuthority && base.inputBank && base.fixedAge >= 0.2f)
            {
                if (base.inputBank.sprint.wasDown)
                {
                    base.characterBody.isSprinting = true;
                    if (this.zooming)
                    {
                        base.cameraTargetParams.cameraParams = Modules.CameraParams.defaultCameraParamsArbiter;
                        aimRequest?.Dispose();
                    }
                    this.RefundCooldown();
                    this.outer.SetNextStateToMain();
                    return;
                }
            }

            if (base.isAuthority && !base.IsKeyDownAuthority() && charge >= 1f)
            {
                BaseCastChanneledSpellState nextState = this.GetNextState();
                if (this.areaIndicatorInstance)
                {
                    if (!this.areaIndicatorInstance.activeSelf)
                    {
                        nextState.spellPosition = Vector3.zero;
                        nextState.spellRotation = Quaternion.identity;
                    }
                    else
                    {
                        nextState.spellPosition = this.areaIndicatorInstance.transform.position;
                        nextState.spellRotation = this.areaIndicatorInstance.transform.rotation;
                    }
                }
                else
                {
                    nextState.spellPosition = base.transform.position;
                    nextState.spellRotation = this.areaIndicatorInstance.transform.rotation;
                }
                nextState.chainActivatorSkillSlot = base.activatorSkillSlot;
                this.outer.SetNextState(nextState);
            }
        }

        private void RefundCooldown()
        {
            base.activatorSkillSlot.rechargeStopwatch = (0.9f * base.activatorSkillSlot.finalRechargeInterval);
        }

        public override void Update()
        {
            base.Update();
            this.UpdateAreaIndicator();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
