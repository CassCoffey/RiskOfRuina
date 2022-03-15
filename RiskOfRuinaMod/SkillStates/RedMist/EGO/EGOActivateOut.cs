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
    class EGOActivateOut : BaseSkillState
    {
        public static float baseDuration = 1f;

        public static float shockwaveRadius = 15f;
        public static float shockwaveForce = 8000f;
        public static float shockwaveBonusForce = 1500f;

        private float duration;
        private RedMistEmotionComponent EGOController;
        private RedMistStatTracker statTracker;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = EGOActivateOut.baseDuration;
            this.EGOController = this.gameObject.GetComponent<RedMistEmotionComponent>();
            this.statTracker = this.gameObject.GetComponent<RedMistStatTracker>();

            base.PlayAnimation("FullBody, Override", "BufferEmpty");
            base.PlayAnimation("FullBody, Override", "EGOActivateOut", "EGOActivate.playbackRate", this.duration);

            if (NetworkServer.active)
            {
                base.characterBody.AddBuff(Modules.Buffs.EGOBuff);
            }

            base.cameraTargetParams.cameraParams = Modules.CameraParams.EGOActivateOutCameraParamsRedMist;
            base.cameraTargetParams.aimMode = CameraTargetParams.AimType.Aura;

            this.FireShockwave();

            // Gotta add new skills
            if (base.skillLocator.utility.baseSkill == Modules.Survivors.RedMist.NormalBlock)
            {
                base.skillLocator.utility.SetSkillOverride(base.skillLocator.utility, Modules.Survivors.RedMist.EGOBlock, GenericSkill.SkillOverridePriority.Contextual);
            }
            else if (base.skillLocator.utility.baseSkill == Modules.Survivors.RedMist.NormalDodge)
            {
                base.skillLocator.utility.SetSkillOverride(base.skillLocator.utility, Modules.Survivors.RedMist.EGODodge, GenericSkill.SkillOverridePriority.Contextual);
            }
                
            base.skillLocator.special.SetSkillOverride(base.skillLocator.special, Modules.Survivors.RedMist.HorizontalSlash, GenericSkill.SkillOverridePriority.Contextual);
        }

        private void FireShockwave()
        {
            Util.PlaySound("Play_Effect_Index_Unlock", base.gameObject);

            EffectData effectData = new EffectData();
            effectData.origin = base.characterBody.corePosition;
            effectData.scale = 1;
            EffectManager.SpawnEffect(statTracker.EGOActivatePrefab, effectData, false);

            if (base.isAuthority)
            {
                BlastAttack blastAttack = new BlastAttack();
                blastAttack.attacker = base.gameObject;
                blastAttack.inflictor = base.gameObject;
                blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
                blastAttack.position = base.characterBody.corePosition;
                blastAttack.procCoefficient = 0f;
                blastAttack.radius = EGOActivateOut.shockwaveRadius;
                blastAttack.baseForce = EGOActivateOut.shockwaveForce;
                blastAttack.bonusForce = Vector3.up * EGOActivateOut.shockwaveBonusForce;
                blastAttack.baseDamage = 0f;
                blastAttack.falloffModel = BlastAttack.FalloffModel.None;
                blastAttack.damageColorIndex = DamageColorIndex.Item;
                blastAttack.attackerFiltering = AttackerFiltering.NeverHitSelf;
                blastAttack.Fire();
            }

            if (this.EGOController) this.EGOController.EnterEGO();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            base.characterMotor.velocity = Vector3.zero;

            if (base.isAuthority && base.fixedAge >= this.duration)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            base.cameraTargetParams.cameraParams = Modules.CameraParams.defaultCameraParamsRedMist;
            base.cameraTargetParams.aimMode = CameraTargetParams.AimType.Standard;

            if (NetworkServer.active) base.characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}
