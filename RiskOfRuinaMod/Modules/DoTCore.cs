using System;
using System.Collections.Generic;
using System.Text;
using R2API;
using R2API.Utils;
using RoR2;

namespace RiskOfRuinaMod.Modules
{
    [R2APISubmoduleDependency(new string[]
    {
        "DotAPI"
    })]
	internal class DoTCore
	{
		internal static DotController.DotIndex FairyIndex;

		public DoTCore()
		{
			this.RegisterDoTs();
		}

		protected internal void RegisterDoTs()
		{
			DotController.DotDef dotDef = new DotController.DotDef()
			{
				interval = 1f,
				damageCoefficient = 0f,
				damageColorIndex = DamageColorIndex.Bleed,
				associatedBuff = Modules.Buffs.fairyDebuff
			};
			DoTCore.FairyIndex = DotAPI.RegisterDotDef(dotDef);
		}
	}
}
