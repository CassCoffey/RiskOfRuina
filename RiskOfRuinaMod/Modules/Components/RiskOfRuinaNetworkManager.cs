using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskOfRuinaMod.Modules.Components
{
    class RiskOfRuinaNetworkManager : NetworkBehaviour
    {
        private static RiskOfRuinaNetworkManager _instance;

        private void Awake()
        {
            _instance = this;
        }

        public static void SetInvisible(GameObject target)
        {
            if (!RiskOfRuinaPlugin._centralNetworkObjectSpawned)
            {
                RiskOfRuinaPlugin._centralNetworkObjectSpawned =
                    Object.Instantiate(RiskOfRuinaPlugin.CentralNetworkObject);
                NetworkServer.Spawn(RiskOfRuinaPlugin._centralNetworkObjectSpawned);
            }
            _instance.RpcSetInvisible(target);
        }

        [ClientRpc]
        private void RpcSetInvisible(GameObject target)
        {
            if (target && target.GetComponent<CharacterBody>() && target.GetComponent<CharacterBody>().modelLocator && target.GetComponent<CharacterBody>().modelLocator.modelTransform && target.GetComponent<CharacterBody>().modelLocator.modelTransform.GetComponent<CharacterModel>())
            {
                target.GetComponent<CharacterBody>().modelLocator.modelTransform.GetComponent<CharacterModel>().invisibilityCount++;
            }
        }
    }
}
