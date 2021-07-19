using EntityStates;
using EntityStates.Mage;
using RiskOfRuinaMod.Modules;
using RiskOfRuinaMod.Modules.Components;
using RiskOfRuinaMod.SkillStates.BaseStates;
using RoR2;
using RoR2.Audio;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskOfRuinaMod.SkillStates
{
    class BlockCounter : BaseSkillState
    {
        public float damageCounter = 0f;
        public int hits = 0;
        public float duration = 0.5f;

        protected RedMistEmotionComponent emotionComponent;
        protected RedMistStatTracker statTracker;

        protected BlastAttack attack;


        public override void OnEnter()
        {
            this.emotionComponent = base.gameObject.GetComponent<RedMistEmotionComponent>();
            this.statTracker = base.gameObject.GetComponent<RedMistStatTracker>();

            if (NetworkServer.active)
            {
                base.characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
            }

            if (base.isAuthority)
            {
                attack = new BlastAttack();
                attack.damageType = DamageType.Generic;
                attack.procCoefficient = 1f;
                attack.baseForce = 300f;
                attack.bonusForce = Vector3.zero;
                attack.baseDamage = damageCounter * StaticValues.blockCounterDamageCoefficient;
                attack.crit = base.RollCrit();
                attack.attacker = this.characterBody.gameObject;
                attack.damageColorIndex = DamageColorIndex.Default;
                attack.falloffModel = BlastAttack.FalloffModel.None;
                attack.radius = 15 + Mathf.Clamp(hits, 0, 15);
                attack.inflictor = this.characterBody.gameObject;
                attack.position = this.characterBody.footPosition;
                attack.procCoefficient = 1.0f;
                attack.teamIndex = TeamComponent.GetObjectTeam(this.characterBody.gameObject);

                attack.Fire();
            }

            base.OnEnter();

            Util.PlaySound("Play_Kali_Normal_Hori", base.gameObject);
            base.PlayAnimation("FullBody, Override", "BlockCounter");

            EffectData effectData = new EffectData();
            effectData.rotation = Quaternion.identity;
            effectData.origin = this.characterBody.footPosition;
            EffectManager.SpawnEffect(statTracker.spinPrefab, effectData, true);

            if (hits > 5)
            {
                Util.PlaySound("Play_Kali_Special_Vert_Fin", base.gameObject);

                effectData = new EffectData();
                effectData.rotation = Quaternion.identity;
                effectData.origin = this.characterBody.footPosition;
                EffectManager.SpawnEffect(statTracker.spinPrefabTwo, effectData, true);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            if (NetworkServer.active && base.characterBody.HasBuff(RoR2Content.Buffs.HiddenInvincibility))
            {
                base.characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
            }

            base.OnExit();
        }
    }
}
