using EntityStates;
using RiskOfRuinaMod.Modules;
using RiskOfRuinaMod.Modules.Components;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RiskOfRuinaMod.SkillStates
{
    class AirBackFallingAttack : BaseSkillState
    {
        private float duration = 10f;
		private float cooldown = 0.4f;
		private float landTime = 0;
		private bool landed = false;
		public ShakeEmitter shakeEmitter;

		private float startY = 0f;

		protected float trueDamage
		{
			get { return (1.0f + (Config.redMistBuffDamage.Value * (float)this.characterBody.GetBuffCount(Modules.Buffs.RedMistBuff))) * (this.damageStat + (this.GetComponent<RedMistStatTracker>().DifferenceAttackSpeed * Config.attackSpeedMult.Value) + (this.GetComponent<RedMistStatTracker>().DifferenceMoveSpeed * Config.moveSpeedMult.Value)); }
		}

		public override void OnEnter()
        {
            base.OnEnter();

            base.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
            if (base.isAuthority)
            {
                base.characterMotor.velocity.y = -60f;
                CharacterMotor characterMotor = base.characterMotor;
                characterMotor.rootMotion.y = characterMotor.rootMotion.y - 0.5f;
                base.characterMotor.velocity.x = 0f;
                base.characterMotor.velocity.z = 0f;
            }

			startY = base.characterBody.corePosition.y;

			base.PlayCrossfade("FullBody, Override", "AirBackSlashContinue", "BaseAttack.playbackRate", this.duration, 0.1f);
        }

		public override void FixedUpdate()
		{
			base.FixedUpdate();

			if (landed)
            {
				if (base.fixedAge >= landTime + this.cooldown && base.isAuthority)
                {
					this.outer.SetNextStateToMain();
                }
			} else
            {
				if (base.isAuthority)
				{
					if (base.characterMotor.velocity.y > -100f)
					{
						base.characterMotor.velocity.y = -100f;
					}
				}
				if ((base.fixedAge >= this.duration || base.characterMotor.isGrounded))
				{
					Util.PlaySound("Play_Kali_Special_Vert_Fin", base.gameObject);
					base.PlayCrossfade("FullBody, Override", "AirBackSlashFinish", "BaseAttack.playbackRate", this.cooldown, 0.1f);

					landed = true;
					landTime = base.fixedAge;

					if (base.isAuthority)
                    {
						shakeEmitter = base.gameObject.AddComponent<ShakeEmitter>();
						shakeEmitter.amplitudeTimeDecay = true;
						shakeEmitter.duration = 0.2f;
						shakeEmitter.radius = 80f;
						shakeEmitter.scaleShakeRadiusWithLocalScale = false;

						shakeEmitter.wave = new Wave
						{
							amplitude = 0.8f,
							frequency = 30f,
							cycleOffset = 0f
						};

						float num = 8f;
						base.AddRecoil(-0.4f * num, -0.8f * num, -0.3f * num, 0.3f * num);
						base.characterMotor.velocity *= 0.1f;
						base.characterBody.bodyFlags--;

						EffectData effectData = new EffectData();
						effectData.origin = base.characterBody.footPosition;
						effectData.scale = 1;
						EffectManager.SpawnEffect(this.GetComponent<RedMistStatTracker>().groundPoundEffect, effectData, true);

						float currentY = base.characterBody.corePosition.y;
						float difY = startY - currentY;
						float mult = Mathf.Clamp(difY / 10f, 1f, 10f);

						Vector3 footPosition = base.characterBody.footPosition;
						BlastAttack blastAttack = new BlastAttack
						{
							radius = 10f + mult,
							procCoefficient = 0.8f,
							position = footPosition,
							attacker = base.gameObject,
							teamIndex = base.teamComponent.teamIndex,
							crit = base.RollCrit(),
							baseDamage = this.trueDamage * StaticValues.airBackSlamAttackDamageCoefficient * mult,
							damageColorIndex = 0,
							falloffModel = 0,
							attackerFiltering = AttackerFiltering.NeverHit,
							damageType = DamageType.Generic
						};
						blastAttack.Fire();
						base.characterMotor.velocity.y = 0f;
					}
				}
			}
		}

        public override void OnExit()
        {
			base.OnExit();
			RiskOfRuinaPlugin.Destroy(shakeEmitter);
		}
    }
}
