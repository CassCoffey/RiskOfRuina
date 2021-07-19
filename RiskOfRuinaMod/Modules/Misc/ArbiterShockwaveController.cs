using R2API;
using RoR2;
using RoR2.Projectile;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskOfRuinaMod.Modules.Misc
{
    [RequireComponent(typeof(TeamFilter))]
    public class ArbiterShockwaveController : NetworkBehaviour
    {
        [SyncVar]
        [Tooltip("The area of effect.")]
        public float radius;
        [Tooltip("The buff type to grant")]
        public BuffDef buffDef;
        [Tooltip("The buff duration")]
        public float buffDuration;
        [Tooltip("Barrier amount (based on percentage)")]
        public float barrierAmount;
        [Tooltip("If set, destroys all projectiles in the vicinity.")]
        public bool destroyProjectiles;
        private TeamFilter teamFilter;

        private void Awake()
        {
            this.teamFilter = base.GetComponent<TeamFilter>();

        }

        private void Start()
        {
            if (NetworkServer.active)
            {
                float radiusSqr = this.radius * this.radius;

                Vector3 position = base.transform.position;
                for (TeamIndex teamIndex = TeamIndex.Neutral; teamIndex < TeamIndex.Count; teamIndex += 1)
                {
                    if (teamIndex != this.teamFilter.teamIndex)
                    {
                        this.HarmTeam(TeamComponent.GetTeamMembers(teamIndex), radiusSqr, position);
                    }
                }
                this.HelpTeam(TeamComponent.GetTeamMembers(this.teamFilter.teamIndex), radiusSqr, position);
            }

            if (this.destroyProjectiles)
            {
                this.DestroyProjectiles(this.radius * this.radius, base.transform.position);
            }
        }

        private void HarmTeam(IEnumerable<TeamComponent> recipients, float radiusSqr, Vector3 currentPosition)
        {
            if (!NetworkServer.active)
            {
                return;
            }

            foreach (TeamComponent teamComponent in recipients)
            {
                if ((teamComponent.transform.position - currentPosition).sqrMagnitude <= radiusSqr)
                {
                    CharacterBody charBody = teamComponent.body;
                    if (charBody)
                    {
                        if (this.buffDef) charBody.AddTimedBuff(this.buffDef, this.buffDuration);
                    }
                }
            }
        }

        private void HelpTeam(IEnumerable<TeamComponent> recipients, float radiusSqr, Vector3 currentPosition)
        {
            if (!NetworkServer.active)
            {
                return;
            }

            foreach (TeamComponent teamComponent in recipients)
            {
                if ((teamComponent.transform.position - currentPosition).sqrMagnitude <= radiusSqr)
                {
                    CharacterBody charBody = teamComponent.body;
                    if (charBody)
                    {
                        if (barrierAmount != 0 && charBody.healthComponent)
                        {
                            charBody.healthComponent.AddBarrier(this.barrierAmount * charBody.healthComponent.fullBarrier);
                        }
                    }
                }
            }
        }

        private void DestroyProjectiles(float radiusSqr, Vector3 currentPosition)
        {
            Collider[] projectiles = Physics.OverlapSphere(currentPosition, radiusSqr, LayerIndex.projectile.mask);

            for (int i = 0; i < projectiles.Length; i++)
            {
                ProjectileController projectile = projectiles[i].GetComponent<ProjectileController>();
                if (projectile)
                {
                    TeamComponent projectileTeam = projectile.owner.GetComponent<TeamComponent>();
                    if (projectileTeam)
                    {
                        if (projectileTeam.teamIndex != this.teamFilter.teamIndex)
                        {
                            EffectData effectData = new EffectData();
                            effectData.origin = projectile.transform.position;
                            effectData.scale = 4;

                            EffectManager.SpawnEffect(Modules.Assets.fairyDeleteEffect, effectData, false);

                            Destroy(projectile.gameObject);
                        }
                    }
                }
            }
        }

        public override bool OnSerialize(NetworkWriter writer, bool forceAll)
        {
            if (forceAll)
            {
                writer.Write(this.radius);
                return true;
            }

            bool flag = false;

            if ((base.syncVarDirtyBits & 1U) != 0U)
            {
                if (!flag)
                {
                    writer.WritePackedUInt32(base.syncVarDirtyBits);
                    flag = true;
                }

                writer.Write(this.radius);
            }

            if (!flag)
            {
                writer.WritePackedUInt32(base.syncVarDirtyBits);
            }
            return flag;
        }

        public override void OnDeserialize(NetworkReader reader, bool initialState)
        {
            if (initialState)
            {
                this.radius = reader.ReadSingle();
                return;
            }

            int num = (int)reader.ReadPackedUInt32();
            if ((num & 1) != 0)
            {
                this.radius = reader.ReadSingle();
            }
        }
    }
}
