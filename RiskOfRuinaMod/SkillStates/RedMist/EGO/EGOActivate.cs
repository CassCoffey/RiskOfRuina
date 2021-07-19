using EntityStates;
using RiskOfRuinaMod.Modules.Components;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskOfRuinaMod.SkillStates
{
    class EGOActivate : BaseSkillState
    {
        public static float baseDuration = 1f;

        private float duration;
        private Vector3 storedPosition;
        private Animator modelAnimator;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = EGOActivate.baseDuration;
            this.modelAnimator = base.GetModelAnimator();
            base.characterBody.hideCrosshair = true;

            if (this.modelAnimator)
            {
                this.modelAnimator.SetBool("isMoving", false);
                this.modelAnimator.SetBool("isSprinting", false);
            }

            if (NetworkServer.active) base.characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);

            foreach (EntityStateMachine i in base.gameObject.GetComponents<EntityStateMachine>())
            {
                if (i)
                {
                    if (i.customName == "Weapon")
                    {
                        i.SetNextStateToMain();
                    }
                    if (i.customName == "Slide")
                    {
                        i.SetNextStateToMain();
                    }
                }
            }

            RedMistEmotionComponent EGOComponent = base.gameObject.GetComponent<RedMistEmotionComponent>();

            base.PlayAnimation("Gesture, Override", "BufferEmpty");
            base.PlayAnimation("FullBody, Override", "EGOActivate", "EGOActivate.playbackRate", this.duration);
            Util.PlaySound("Play_Kali_Change", base.gameObject);

            base.cameraTargetParams.cameraParams = Modules.CameraParams.EGOActivateCameraParamsRedMist;

            this.storedPosition = base.transform.position;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            base.transform.position = this.storedPosition;
            base.characterBody.isSprinting = false;
            if (base.characterMotor) base.characterMotor.velocity = Vector3.zero;

            if (base.isAuthority && base.fixedAge >= this.duration)
            {
                this.outer.SetNextState(new EGOActivateOut());
                return;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}
