using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RiskOfRuinaMod.Modules.Items
{
    class MoonlightStoneBehavior : CharacterBody.ItemBehavior
    {
		private float timer;

		public void Awake()
		{
			this.body = base.gameObject.GetComponent<CharacterBody>();
		}

		public void FixedUpdate()
		{
			this.timer += Time.deltaTime;
			if (this.timer >= 2f)
			{
				int removed = 0;

                DotController selfDotController = DotController.FindDotController(body.gameObject);
                if (selfDotController)
                {
                    for (int i = selfDotController.dotStackList.Count - 1; i >= 0 && removed < this.stack; i--)
                    {
                        selfDotController.RemoveDotStackAtServer(i);
						removed++;
                    }
                }

                for (int i = body.activeBuffsList.Length - 1; i >= 0 && removed < this.stack; i--)
                {
					BuffDef buff = BuffCatalog.GetBuffDef(body.activeBuffsList[i]);
					if (buff.isDebuff)
                    {
						body.RemoveBuff(body.activeBuffsList[i]);
						removed++;
                    }
                }
				this.timer = 0f;
			}
		}
		
	}
}
