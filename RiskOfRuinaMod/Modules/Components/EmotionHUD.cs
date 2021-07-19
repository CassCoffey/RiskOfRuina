using System;
using RoR2;
using RoR2.UI;
using UnityEngine;
using UnityEngine.UI;

namespace RiskOfRuinaMod.Modules.Components
{
	public class EmotionHUD : MonoBehaviour
	{
		public GameObject emotionGauge;
		public Image emotionFill;
		private Color startColor;
		private Color endColor;
		private float currentFill;
		private HUD hud;

		private void Awake()
		{
			this.hud = base.GetComponent<HUD>();
			this.startColor = new Color(171f, 115f, 10f);
			this.endColor = new Color(236f, 82f, 0f);
		}

		private void FillGauge(float desiredFill)
		{
			if (desiredFill > this.currentFill)
			{
				this.currentFill += 15f * Time.deltaTime;
				if (this.currentFill > desiredFill)
				{
					this.currentFill = desiredFill;
				}
			}
			else
			{
				this.currentFill -= 15f * Time.deltaTime;
				if (this.currentFill < desiredFill)
				{
					this.currentFill = desiredFill;
				}
			}
		}

		public void Update()
		{
			if (this.hud.targetBodyObject)
			{
				RedMistEmotionComponent component = this.hud.targetBodyObject.GetComponent<RedMistEmotionComponent>();
				if (component)
				{
					PlayerCharacterMasterController playerCharacterMasterController = this.hud.targetMaster ? this.hud.targetMaster.playerCharacterMasterController : null;
					if (this.emotionGauge)
					{
						this.emotionGauge.gameObject.SetActive(true);
						float desiredFill = component.currentEmotion / component.maxEmotion;
						float fillAmount = this.emotionFill.fillAmount;
						this.FillGauge(desiredFill);
						this.emotionFill.fillAmount = this.currentFill;
						float num = Mathf.Lerp(this.startColor.r, this.endColor.r, this.currentFill);
						float num2 = Mathf.Lerp(this.startColor.g, this.endColor.g, this.currentFill);
						float num3 = Mathf.Lerp(this.startColor.b, this.endColor.b, this.currentFill);
						Color cyan = new Color(num, num2, num3);
						if (this.currentFill >= 1f)
						{
							cyan = Color.cyan;
						}
						this.emotionFill.color = cyan;
					}
				}
				else
				{
					if (this.emotionGauge)
					{
						this.emotionGauge.gameObject.SetActive(false);
					}
				}
			}
		}
	}
}
