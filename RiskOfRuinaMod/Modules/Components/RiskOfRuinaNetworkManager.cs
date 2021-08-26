using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskOfRuinaMod.Modules.Components
{
    class RiskOfRuinaNetworkManager : NetworkBehaviour
    {
        private static RiskOfRuinaNetworkManager _instance;

        public static event hook_ServerOnHit ServerOnHit;
        public delegate void hook_ServerOnHit(float damage, UnityEngine.GameObject attacker, UnityEngine.GameObject victim);

        private void Awake()
        {
            _instance = this;
        }

        public static void SetInvisible(GameObject target)
        {
            if (!RiskOfRuinaPlugin._centralNetworkObjectSpawned)
            {
                RiskOfRuinaPlugin._centralNetworkObjectSpawned =
                    UnityEngine.Object.Instantiate(RiskOfRuinaPlugin.CentralNetworkObject);
                NetworkServer.Spawn(RiskOfRuinaPlugin._centralNetworkObjectSpawned);
            }
            _instance.RpcSetInvisible(target);
        }

        public static void OnHit(GlobalEventManager self, DamageInfo damageInfo, UnityEngine.GameObject victim)
        {
            if (!RiskOfRuinaPlugin._centralNetworkObjectSpawned)
            {
                RiskOfRuinaPlugin._centralNetworkObjectSpawned =
                    UnityEngine.Object.Instantiate(RiskOfRuinaPlugin.CentralNetworkObject);
                NetworkServer.Spawn(RiskOfRuinaPlugin._centralNetworkObjectSpawned);
            }
            _instance.RpcOnHitInvoke(damageInfo.damage, damageInfo.attacker, victim);
        }

        [ClientRpc]
        private void RpcSetInvisible(GameObject target)
        {
            if (target && target.GetComponent<CharacterBody>() && target.GetComponent<CharacterBody>().modelLocator && target.GetComponent<CharacterBody>().modelLocator.modelTransform && target.GetComponent<CharacterBody>().modelLocator.modelTransform.GetComponent<CharacterModel>())
            {
                target.GetComponent<CharacterBody>().modelLocator.modelTransform.GetComponent<CharacterModel>().invisibilityCount++;
            }
        }

        [ClientRpc]
        private void RpcOnHitInvoke(float damage, UnityEngine.GameObject attacker, UnityEngine.GameObject victim)
        {
            hook_ServerOnHit handler = ServerOnHit;
            handler?.Invoke(damage, attacker, victim);
        }
    }
}
