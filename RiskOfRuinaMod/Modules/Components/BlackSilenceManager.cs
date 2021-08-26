using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskOfRuinaMod.Modules.Components
{
    public class BlackSilenceManager : NetworkBehaviour
	{
		public CharacterBody characterBody;

		public bool angelica = false;

		private void Start()
		{
			this.characterBody = base.GetComponent<CharacterBody>();
		}

		// this will handle spawning and tracking weapons
	}
}
