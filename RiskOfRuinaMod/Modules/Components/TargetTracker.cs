using System;
using System.Linq;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskOfRuinaMod.Modules.Components
{
	public class TargetTracker : NetworkBehaviour
	{
		public float maxTrackingDistance = 60f;

		public float maxTrackingAngle = 45f;

		public float trackerUpdateFrequency = 10f;

		public bool trackEnemy = true;

		private CharacterBody trackingTarget;

		private CharacterBody characterBody;

		private TeamComponent teamComponent;

		private InputBankTest inputBank;

		private float trackerUpdateStopwatch;

		private Indicator indicator;

		private readonly BullseyeSearch search = new BullseyeSearch();

		private TeamMask friendlies;


		private void Awake()
		{
			this.indicator = new Indicator(base.gameObject, Modules.Assets.trackerPrefab);
		}

		private void Start()
		{
			this.characterBody = base.GetComponent<CharacterBody>();
			this.inputBank = base.GetComponent<InputBankTest>();
			this.teamComponent = base.GetComponent<TeamComponent>();

			friendlies = new TeamMask();
			friendlies.AddTeam(this.teamComponent.teamIndex);

			if (characterBody.skillLocator.secondary.skillDef == Skills.unlockSkillDef)
            {
				trackEnemy = false;
            }
		}

		public CharacterBody GetTrackingTarget()
		{
			return this.trackingTarget;
		}

		private void OnEnable()
		{
			this.indicator.active = true;
		}

		private void OnDisable()
		{
			this.indicator.active = false;
		}

		private void OnDestroy()
		{
			this.indicator.active = false;
		}

		private void FixedUpdate()
		{
			if (!hasAuthority)
				return;

			this.trackerUpdateStopwatch += Time.fixedDeltaTime;
			if (this.trackerUpdateStopwatch >= 1f / this.trackerUpdateFrequency)
			{
				this.trackerUpdateStopwatch -= 1f / this.trackerUpdateFrequency;
				Ray aimRay = new Ray(this.inputBank.aimOrigin, this.inputBank.aimDirection);
				this.SearchForTarget(aimRay);
				this.indicator.targetTransform = (this.trackingTarget ? this.trackingTarget.transform : null);
			}
		}

		private void SearchForTarget(Ray aimRay)
		{
			if (trackEnemy)
            {
				this.search.teamMaskFilter = TeamMask.GetUnprotectedTeams(this.teamComponent.teamIndex);
			} else
            {
				this.search.teamMaskFilter = friendlies;
			}
			
			this.search.filterByLoS = true;
			this.search.searchOrigin = aimRay.origin;
			this.search.searchDirection = aimRay.direction;
			this.search.sortMode = RoR2.BullseyeSearch.SortMode.Angle;
			this.search.maxDistanceFilter = this.maxTrackingDistance;
			this.search.maxAngleFilter = this.maxTrackingAngle;
			this.search.RefreshCandidates();
			this.search.FilterOutGameObject(base.gameObject);
			HurtBox candidate = this.search.GetResults().FirstOrDefault<HurtBox>();
			if (candidate && candidate.healthComponent && candidate.healthComponent.body)
			{
				if (candidate.healthComponent.body != this.trackingTarget)
                {
					CmdSetTarget(candidate.healthComponent.body.gameObject);
				}
			} else
            {
				if (this.trackingTarget != null)
                {
					CmdSetTarget(null);
				}
            }
		}

		[Command]
		public void CmdSetTarget(GameObject targetNet)
		{
			RpcSetTarget(targetNet);
		}

		[ClientRpc]
		public void RpcSetTarget(GameObject targetNet)
		{
			if (targetNet && targetNet.GetComponent<CharacterBody>())
			{
				this.trackingTarget = targetNet.GetComponent<CharacterBody>();
			} else
            {
				this.trackingTarget = null;
			}
		}
	}
}
