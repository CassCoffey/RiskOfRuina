using BepInEx.Configuration;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiskOfRuinaMod.Modules.Items
{
    public abstract class RuinaItem
    {
        internal abstract ConfigEntry<bool> itemEnabled { get; set; }
        internal abstract string itemName { get; set; }

        public virtual void Init()
        {
            itemEnabled = Modules.Config.ItemEnableConfig(itemName);

            if (itemEnabled.Value)
            {
                this.ItemSetup();
                this.HookSetup();
            }
        }

        public abstract void ItemSetup();

        public abstract void HookSetup();

        public int GetCount(CharacterBody character)
        {
            int result = 0;
            if (character && character.inventory)
            {
                result = character.inventory.GetItemCount(ItemCatalog.FindItemIndex(itemName));
            }
            return result;
        }
    }
}
