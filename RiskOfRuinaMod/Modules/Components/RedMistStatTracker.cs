using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskOfRuinaMod.Modules.Components
{
    public class RedMistStatTracker : NetworkBehaviour
	{
		public CharacterBody characterBody;

		public bool argalia = false;

		public GameObject slashPrefab = Assets.swordSwingEffect;
		public GameObject piercePrefab = Assets.spearPierceEffect;
		public GameObject spinPrefab = Assets.swordSpinEffect;
		public GameObject spinPrefabTwo = Assets.swordSpinEffectTwo;
		public GameObject EGOSlashPrefab = Assets.EGOSwordSwingEffect;
		public GameObject EGOPiercePrefab = Assets.EGOSpearPierceEffect;
		public GameObject EGOHorizontalPrefab = Assets.HorizontalSwordSwingEffect;
		public GameObject EGOActivatePrefab = Assets.EGOActivate;
		public GameObject hitEffect = Assets.swordHitEffect;
		public GameObject phaseEffect = Assets.phaseEffect;
		public GameObject groundPoundEffect = Assets.groundPoundEffect;
		public GameObject afterimageSlash = Assets.afterimageSlash;
		public GameObject afterimageBlock = Assets.afterimageBlock;
		public GameObject counterBurst = Assets.counterBurst;

		public float totalAttackSpeed = StaticValues.originalAttackSpeed;
		public float totalMoveSpeed = StaticValues.originalMoveSpeed;
		public float lastAttackSpeed = StaticValues.originalAttackSpeed;
		public float lastMoveSpeed = StaticValues.originalMoveSpeed;
		public float modifiedAttackSpeed = StaticValues.originalAttackSpeed;
		public float modifiedMoveSpeed = StaticValues.originalMoveSpeed;

		public ParticleSystem mistEffect;

		public string musicName = "Play_Ruina_Boss_Music";

		public float DifferenceMoveSpeed
		{
			get { return Mathf.Clamp(totalMoveSpeed - modifiedMoveSpeed, 0f, totalMoveSpeed); }
		}

		public float DifferenceAttackSpeed
		{
			get { return Mathf.Clamp(totalAttackSpeed - modifiedAttackSpeed, 0f, totalAttackSpeed); }
		}

		private void Start()
		{
			this.characterBody = base.GetComponent<CharacterBody>();

			ChildLocator childLocator = this.gameObject.GetComponentInChildren<ChildLocator>();
			if (childLocator)
			{
				mistEffect = childLocator.FindChild("BloodCloud").GetComponent<ParticleSystem>();
			}

			argalia = (this.characterBody.skinIndex == RiskOfRuinaPlugin.argaliaSkinIndex);

			if (argalia)
            {
				musicName = "Play_ArgaliaMusic";

				slashPrefab = Assets.argaliaSwordSwingEffect;
				piercePrefab = Assets.argaliaSpearPierceEffect;
				EGOSlashPrefab = Assets.argaliaEGOSwordSwingEffect;
				EGOPiercePrefab = Assets.argaliaEGOSpearPierceEffect;
				EGOHorizontalPrefab = Assets.argaliaHorizontalSwordSwingEffect;
				EGOActivatePrefab = Assets.argaliaEGOActivate;
				hitEffect = Assets.argaliaSwordHitEffect;
				phaseEffect = Assets.argaliaPhaseEffect;
				groundPoundEffect = Assets.argaliaGroundPoundEffect;
				spinPrefab = Assets.argaliaSwordSpinEffect;
				spinPrefabTwo = Assets.argaliaSwordSpinEffectTwo;
				counterBurst = Assets.argaliaCounterBurst;
				afterimageBlock = Assets.argaliaAfterimageBlock;
				afterimageSlash = Assets.argaliaAfterimageSlash;

				if (childLocator)
				{
					mistEffect = childLocator.FindChild("ArgaliaCloud").GetComponent<ParticleSystem>();

					childLocator.FindChild("ParticleHair").GetChild(0).gameObject.SetActive(false);
					childLocator.FindChild("ParticleHair").GetChild(1).gameObject.SetActive(true);
				}
			}
		}

		public float CalculateMoveSpeed(float moveSpeed)
        {
			float newMoveSpeed = moveSpeed - lastMoveSpeed;

			totalMoveSpeed += newMoveSpeed;
			modifiedMoveSpeed += newMoveSpeed * Config.statRatio.Value;

			lastMoveSpeed = moveSpeed;

			return modifiedMoveSpeed;
		}

		public float CalculateAttackSpeed(float attackSpeed)
		{
			float newAttackSpeed = attackSpeed - lastAttackSpeed;

			totalAttackSpeed += newAttackSpeed;
			modifiedAttackSpeed += newAttackSpeed * Config.statRatio.Value;

			lastAttackSpeed = attackSpeed;

			return modifiedAttackSpeed;
		}
	}
}
