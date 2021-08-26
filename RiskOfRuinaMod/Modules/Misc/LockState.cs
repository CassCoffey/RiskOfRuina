using System;
using System.Collections.Generic;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace RiskOfRuinaMod.Modules.Misc
{
    class LockState : BaseState
    {
		public float duration = 5f;

		private List<GameObject> lockedProjectiles;
		private List<projectileInfo> lockedProjectileInfo;

		public override void OnEnter()
		{
			if (RiskOfRuinaPlugin.kombatArenaInstalled)
			{
				if (RiskOfRuinaPlugin.KombatGamemodeActive() && characterBody.master && RiskOfRuinaPlugin.KombatIsDueling(characterBody.master) && duration > 3f)
				{
					duration = 3f;
				}
			}

			base.OnEnter();

			lockedProjectiles = new List<GameObject>();
			lockedProjectileInfo = new List<projectileInfo>();

			Animator modelAnimator = base.GetModelAnimator();
			if (modelAnimator)
			{
				modelAnimator.enabled = false;
			}
			if (base.rigidbody && !base.rigidbody.isKinematic)
			{
				base.rigidbody.velocity = Vector3.zero;
				if (base.rigidbodyMotor)
				{
					base.rigidbodyMotor.moveVector = Vector3.zero;
				}
			}
		}

		public override void OnExit()
		{
			Animator modelAnimator = base.GetModelAnimator();
			if (modelAnimator)
			{
				modelAnimator.enabled = true;
			}

			foreach (projectileInfo projectileInfo in lockedProjectileInfo)
			{
				if (projectileInfo.projectile)
				{
					Destroy(projectileInfo.projectile.gameObject);
				}
			}

			base.OnExit();
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();

			if (base.characterMotor)
            {
				base.characterMotor.velocity = Vector3.zero;
			}

			// Try to hold any projectiles they may have been firing.
			Collider[] projectiles = Physics.OverlapSphere(base.transform.position, 50f, LayerIndex.projectile.mask);

			for (int i = 0; i < projectiles.Length; i++)
			{
				ProjectileController projectile = projectiles[i].GetComponent<ProjectileController>();
				if (projectile)
				{
					if (projectile.owner)
					{
						if (projectile.owner == base.gameObject)
						{
							if (!lockedProjectiles.Contains(projectile.gameObject))
                            {
								lockedProjectiles.Add(projectile.gameObject);

								Vector3 velocity = Vector3.zero;
								Vector3 moveVector = Vector3.zero;
								
								Rigidbody rigidbody = projectile.GetComponent<Rigidbody>();
								if (rigidbody && !rigidbody.isKinematic)
								{
									velocity = rigidbody.velocity;
									if (projectile.GetComponent<RigidbodyMotor>())
									{
										moveVector = projectile.GetComponent<RigidbodyMotor>().moveVector;
									}
								}
								
								projectileInfo info = new projectileInfo()
								{
									projectile = projectile.gameObject,
									velocity = velocity,
									moveVector = moveVector,
									position = projectile.gameObject.transform.position
								};
								
								lockedProjectileInfo.Add(info);
							}
						}
					}
				}
			}

			foreach (projectileInfo projectileInfo in lockedProjectileInfo)
			{
				if (projectileInfo.projectile)
				{
					projectileInfo.projectile.transform.position = projectileInfo.position;

					ProjectileController controller = projectileInfo.projectile.GetComponent<ProjectileController>();
					if (controller)
					{
						foreach (Collider collider in controller.myColliders)
                        {
							collider.enabled = false;
						}
					}
					Rigidbody rigidbody = projectileInfo.projectile.GetComponent<Rigidbody>();
					if (rigidbody && !rigidbody.isKinematic)
					{
						rigidbody.velocity = Vector3.zero;
						if (projectileInfo.projectile.GetComponent<RigidbodyMotor>())
						{
							projectileInfo.projectile.GetComponent<RigidbodyMotor>().moveVector = Vector3.zero;
						}
					}
				}
			}

			if (base.fixedAge >= this.duration)
			{
				this.outer.SetNextStateToMain();
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Frozen;
		}

		private struct projectileInfo
        {
			public GameObject projectile;
			public Vector3 velocity;
			public Vector3 moveVector;
			public Vector3 position;
        }
	}
}
