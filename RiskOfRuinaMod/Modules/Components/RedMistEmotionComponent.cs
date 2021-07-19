using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskOfRuinaMod.Modules.Components
{
	public class RedMistEmotionComponent : NetworkBehaviour
	{
		[SyncVar]
		public bool inEGO = false;
		public float maxEmotion = 100f;
		[SyncVar]
		public float currentEmotion;
		[SyncVar]
		private float EGOage = 0f;
		private bool bossMode = false;

		private uint playID;
		private bool isPlaying;

		private bool exitRequested = false;

		private CharacterBody characterBody;
		private Animator modelAnimator;
		private HealthComponent healthComponent;
		private RedMistStatTracker statTracker;

		private void Awake()
		{
			this.characterBody = base.gameObject.GetComponent<CharacterBody>();
			this.modelAnimator = base.gameObject.GetComponentInChildren<Animator>();
			this.healthComponent = characterBody.healthComponent;
			this.statTracker = base.gameObject.GetComponent<RedMistStatTracker>();

			if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("moon")
				|| UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("limbo")
				|| UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("goldshores"))
			{
				this.bossMode = true;
				this.Invoke("SendEGOMessage", 2f);
			}

			this.Invoke("CheckSkill", 0.2f);
			base.InvokeRepeating("CheckAlive", 0.25f, 0.25f);
		}

		private void FixedUpdate()
        {
			if (this.currentEmotion < maxEmotion || inEGO)
            {
				float decay = Config.emotionDecay.Value;
				if (inEGO) decay += EGOage * Config.EGOAgeRatio.Value;
				if (bossMode) decay = 0f;
				this.SpendEmotion(decay);
			}

			if (this.inEGO)
			{
				EGOage += Time.fixedDeltaTime;
				if (hasAuthority && currentEmotion <= 0f && !exitRequested)
                {
					ExitEGO();
					exitRequested = true;
				}
			}

			if (!hasAuthority)
				return;

			if (this.bossMode && this.currentEmotion < maxEmotion)
            {
				CmdAddEmotion(100f);
            }

			if (RiskOfRuinaPlugin.DEBUG_MODE)
            {
				if (Input.GetKeyDown(KeyCode.Z))
                {
					CmdAddEmotion(100f);
				}
            }
		}

		private void CheckAlive()
		{
			if (this.inEGO && this.healthComponent && !this.healthComponent.alive)
			{
				this.DeathExitEGO();
			}
		}

		private void OnDestroy()
		{
			this.StopMusic();
		}

		private void OnDisable()
		{
			this.StopMusic();
		}

		private void SendEGOMessage()
        {
			if (statTracker.argalia)
            {
				RoR2.Chat.AddMessage("<color=#075ed9>This performance will be my final enlightenment.</color>");
				RoR2.Chat.AddMessage("<color=#075ed9>Let's have a dance together.</color>");
			} else
            {
				RoR2.Chat.AddMessage("<color=#ad0e0e>Guess it's time to stop holding back...</color>");
				RoR2.Chat.AddMessage("<color=#ad0e0e>Let me show you how to actually wield EGO.</color>");
			}
		}

		private void CheckSkill()
		{
			if (this.characterBody)
			{
				if (this.characterBody.skillLocator)
				{
					if (this.characterBody.skillLocator.special.skillNameToken != RiskOfRuinaPlugin.developerPrefix + "_REDMIST_BODY_SPECIAL_EGO_NAME")
					{
						Destroy(this);
					}
				}
			}
		}

		public bool AddEmotion()
		{
			return this.AddEmotion(2f);
		}

		public bool SpendEmotion(float amount)
		{
			this.currentEmotion = Mathf.Clamp(this.currentEmotion - amount, 0f, this.maxEmotion);
			return true;
		}

		public bool AddEmotion(float amount)
		{
			if (!isServer)
			{
				return false;
			}

			RpcAddEmotion(amount);

			return true;
		}

		public void EnterEGO()
		{
			this.inEGO = true;
			this.currentEmotion = 100f;
			this.EGOage = 0f;

			if (isServer && this.healthComponent)
			{
				healthComponent.Heal(healthComponent.fullHealth, new ProcChainMask());
			}

			StartMusic();

			if (this.modelAnimator) this.modelAnimator.SetLayerWeight(this.modelAnimator.GetLayerIndex("EGO"), 1f);
		}

		public void ExitEGO()
		{
			CmdExitEgo();
		}

		public void DeathExitEGO()
		{
			CmdDeathExitEgo();
		}

		private void StartMusic()
		{
			if (Config.themeMusic.Value)
			{
				this.playID = Util.PlaySound(statTracker.musicName, base.gameObject);
				Music.musicSources++;
				this.isPlaying = true;
			}
		}

		private void StopMusic()
		{
			if (Config.themeMusic.Value && this.isPlaying)
			{
				this.isPlaying = false;
				AkSoundEngine.PostEvent("StopThemes", base.gameObject);
				//AkSoundEngine.StopPlayingID(this.playID);
				Music.musicSources--;
			}
		}

		[Command]
		public void CmdAddEmotion(float amount)
        {
			RpcAddEmotion(amount);
		}

		[Command]
		public void CmdExitEgo ()
        {
			RpcExitEgo();
		}

		[Command]
		public void CmdDeathExitEgo()
		{
			RpcDeathExitEgo();
		}

		[ClientRpc]
		public void RpcExitEgo()
        {
			this.inEGO = false;
			this.currentEmotion = 0f;
			this.exitRequested = false;

			StopMusic();

			if (isServer)
            {
				characterBody.RemoveBuff(Modules.Buffs.EGOBuff);
				int buffsToRemove = characterBody.GetBuffCount(Modules.Buffs.RedMistBuff);
				for (int i = 0; i < buffsToRemove; i++)
				{
					characterBody.RemoveBuff(Modules.Buffs.RedMistBuff);
				}
			}

			if (this.modelAnimator) this.modelAnimator.SetLayerWeight(this.modelAnimator.GetLayerIndex("EGO"), 0f);

			EntityStateMachine bodyStateMachine = null;
			foreach (EntityStateMachine i in base.gameObject.GetComponents<EntityStateMachine>())
			{
				if (i)
				{
					if (i.customName == "Body")
					{
						bodyStateMachine = i;
					}
				}
			}
			if (bodyStateMachine)
			{
				bodyStateMachine.SetNextState(new SkillStates.EGODeactivate());
			}
		}

		[ClientRpc]
		public void RpcDeathExitEgo()
		{
			this.inEGO = false;
			this.currentEmotion = 0f;
			this.exitRequested = false;

			StopMusic();

			if (this.modelAnimator) this.modelAnimator.SetLayerWeight(this.modelAnimator.GetLayerIndex("EGO"), 0f);
		}

		[ClientRpc]
		public void RpcAddEmotion(float amount)
        {
			if (this.currentEmotion < this.maxEmotion)
			{
				this.currentEmotion = Mathf.Clamp(this.currentEmotion + amount, 0f, this.maxEmotion);
			}
		}
	}
}
