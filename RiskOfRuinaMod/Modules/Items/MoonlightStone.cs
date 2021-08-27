using BepInEx.Configuration;
using R2API;
using RiskOfRuinaMod.Modules.Misc;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskOfRuinaMod.Modules.Items
{
    class MoonlightStone : RuinaItem
    {
        internal override ConfigEntry<bool> itemEnabled { get; set; }
        internal override string itemName { get; set; } = "RuinaMoonlightStone";

        public ItemDef itemDef;

        public override void ItemSetup()
        {
            itemDef = ScriptableObject.CreateInstance<ItemDef>();
            itemDef.name = itemName;
            itemDef.tier = ItemTier.Tier1;
            itemDef.pickupModelPrefab = Assets.moonlightStone;
            itemDef.pickupIconSprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texIconPickupRuinaMoonlightStone");
            itemDef.nameToken = itemName.ToUpper() + "_NAME";
            itemDef.pickupToken = itemName.ToUpper() + "_PICKUP";
            itemDef.descriptionToken = itemName.ToUpper() + "_DESC";
            itemDef.loreToken = itemName.ToUpper() + "_LORE";
            itemDef.tags = new[]
                {
                    ItemTag.Utility
                };

            var itemDisplayRules = new ItemDisplayRule[0];

            var item = new R2API.CustomItem(itemDef, itemDisplayRules);

            ItemAPI.Add(item);
        }

        public override void HookSetup()
        {
            On.RoR2.CharacterBody.FixedUpdate += new On.RoR2.CharacterBody.hook_FixedUpdate(this.ClearBuffs);
        }

		private void ClearBuffs(On.RoR2.CharacterBody.orig_FixedUpdate orig, CharacterBody self)
		{
			if (NetworkServer.active)
			{
                int count = base.GetCount(self);

                if (count > 0)
                {
                    MoonlightStoneTracker stoneTracker = self.GetComponent<MoonlightStoneTracker>();
                    if (!stoneTracker)
                    {
                        stoneTracker = self.gameObject.AddComponent<MoonlightStoneTracker>();
                    }

                    stoneTracker.timer += Time.deltaTime;
                    if (stoneTracker.timer >= 2f)
                    {
                        int removed = 0;

                        DotController selfDotController = DotController.FindDotController(self.gameObject);
                        if (selfDotController)
                        {
                            for (int i = selfDotController.dotStackList.Count - 1; i >= 0 && removed < count; i--)
                            {
                                selfDotController.RemoveDotStackAtServer(i);
                                removed++;
                            }
                        }

                        for (int i = self.activeBuffsList.Length - 1; i >= 0 && removed < count; i--)
                        {
                            BuffDef buff = BuffCatalog.GetBuffDef(self.activeBuffsList[i]);
                            if (buff.isDebuff && self.GetBuffCount(buff) > 0)
                            {
                                // Some buffs are actually debuffs
                                if (buff.buffIndex != BuffCatalog.FindBuffIndex("BanditSkull") && buff.buffIndex != BuffCatalog.FindBuffIndex("ElementalRingsCooldown"))
                                {
                                    self.RemoveBuff(self.activeBuffsList[i]);
                                    removed++;
                                }
                            }
                        }
                        stoneTracker.timer = 0f;
                    }
                }
            }
			orig.Invoke(self);
		}
	}

    public class MoonlightStoneTracker : MonoBehaviour
    {
        public float timer = 0f;
    }
}