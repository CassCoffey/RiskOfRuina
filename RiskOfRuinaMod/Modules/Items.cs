using RiskOfRuinaMod.Modules.Items;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiskOfRuinaMod.Modules
{
    class ItemManager
    {
        public static ItemManager instance;

        public List<RuinaItem> items = new List<RuinaItem>();
        public List<RuinaEquipment> equips = new List<RuinaEquipment>();

        public ItemManager()
        {
            ItemManager.instance = this;
        }

        public void AddItems()
        {
            foreach(RuinaItem ruinaItem in this.items)
            {
                ruinaItem.Init();
            }
        }

        public void AddEquips()
        {
            foreach (RuinaEquipment ruinaEquip in this.equips)
            {
                ruinaEquip.Init();
            }
        }
    }
}
