using BepInEx.Configuration;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiskOfRuinaMod.Modules.Items
{
    public abstract class RuinaEquipment
    {
        internal abstract ConfigEntry<bool> equipEnabled { get; set; }
        internal abstract string equipName { get; set; }

        public virtual void Init()
        {
            equipEnabled = Modules.Config.ItemEnableConfig(equipName);

            if (equipEnabled.Value)
            {
                this.EquipSetup();
                this.HookSetup();
            }
        }

        public abstract void EquipSetup();

        public abstract void HookSetup();
    }
}
