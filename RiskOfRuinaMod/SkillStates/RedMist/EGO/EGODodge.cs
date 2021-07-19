using EntityStates;
using EntityStates.Mage;
using RiskOfRuinaMod.Modules;
using RiskOfRuinaMod.Modules.Components;
using RoR2;
using RoR2.Audio;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskOfRuinaMod.SkillStates
{
    class EGODodge : BaseSkillState
    {
        public Vector3 dodgeVector;
        public float duration = 0.65f;
        public float blinkDuration = 0.3f;
        public float stockBonus = 0.05f;
        public bool aerial = false;
        public bool invul = false;

        private Transform modelTransform;
        private CharacterModel characterModel;
        private Animator animator;
        private HurtBoxGroup hurtboxGroup;
        private RedMistStatTracker statTracker;
        private ParticleSystem mistEffect;


        public override void OnEnter()
        {
            this.modelTransform = base.GetModelTransform();

            if (this.modelTransform)
            {
                this.animator = this.modelTransform.GetComponent<Animator>();
                this.characterModel = this.modelTransform.GetComponent<CharacterModel>();
                this.hurtboxGroup = this.modelTransform.GetComponent<HurtBoxGroup>();
            }

            if (base.skillLocator.utility.stock > 1)
            {
                float stockValue = (base.skillLocator.utility.stock - 1) * stockBonus;
                this.duration = Mathf.Clamp(this.duration - stockValue, blinkDuration, this.duration);
            }

            statTracker = this.GetComponent<RedMistStatTracker>();

            this.dodgeVector = inputBank.moveVector;

            aerial = !this.characterMotor.isGrounded;

            if (this.characterModel)
            {
                this.characterModel.invisibilityCount++;
            }
            if (this.hurtboxGroup)
            {
                HurtBoxGroup hurtBoxGroup = this.hurtboxGroup;
                int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter + 1;
                hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
            }

            Util.PlaySound("Play_Claw_Ulti_Move", base.gameObject);
            base.PlayAnimation("EGODodge", "EGODodge", "Dodge.playbackRate", this.blinkDuration);

            EffectData effectData = new EffectData();
            effectData.rotation = Quaternion.identity;
            effectData.origin = this.characterBody.corePosition;
            EffectManager.SpawnEffect(statTracker.phaseEffect, effectData, true);

            ChildLocator childLocator = base.GetModelChildLocator();
            if (childLocator)
            {
                this.mistEffect = base.GetComponent<RedMistStatTracker>().mistEffect;
                this.mistEffect.Play();
            }

            invul = true;

            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge <= this.blinkDuration)
            {
                base.characterMotor.rootMotion += dodgeVector * (40f * Time.fixedDeltaTime);
                base.characterMotor.moveDirection *= 2f;
            }

            if (base.fixedAge >= this.blinkDuration && invul)
            {
                if (this.characterModel)
                {
                    this.characterModel.invisibilityCount--;
                }
                if (this.hurtboxGroup)
                {
                    HurtBoxGroup hurtBoxGroup = this.hurtboxGroup;
                    int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter - 1;
                    hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
                }
                EffectData effectData = new EffectData();
                effectData.rotation = Quaternion.identity;
                effectData.origin = this.characterBody.corePosition;
                EffectManager.SpawnEffect(statTracker.phaseEffect, effectData, true);
                invul = false;

                this.mistEffect.Stop();
            }

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            this.mistEffect.Stop();

            if (this.characterModel && this.characterModel.invisibilityCount > 0)
            {
                this.characterModel.invisibilityCount--;
            }

            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}
