using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace RiskOfRuinaMod.Modules
{
    public static class Buffs
    {
        internal static BuffDef EGOBuff;
        internal static BuffDef RedMistBuff;

        internal static BuffDef fairyDebuff;
        internal static BuffDef lockDebuff;
        internal static BuffDef lockResistBuff;
        internal static BuffDef feebleDebuff;
        internal static BuffDef strengthBuff;
        internal static BuffDef warpBuff;

        internal static List<BuffDef> buffDefs = new List<BuffDef>();

        internal static void RegisterBuffs()
        {
            EGOBuff = AddNewBuff("EGOBuff", Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texEGOBuffIcon"), Color.white, false, false);
            RedMistBuff = AddNewBuff("RedMistBuff", Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texRedMistBuffIcon"), Color.white, true, false);

            fairyDebuff = AddNewBuff("FairyDebuff", Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texFairyDebuff"), Color.white, true, true);
            lockDebuff = AddNewBuff("LockDebuff", Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texLockDebuff"), Color.white, false, true);
            lockResistBuff = AddNewBuff("LockResistance", Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texEnduringBuff"), Color.white, true, false);
            feebleDebuff = AddNewBuff("FeebleDebuff", Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texFeebleDebuff"), Color.white, false, true);
            strengthBuff = AddNewBuff("StrengthBuff", Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texStrengenedBuff"), Color.white, false, false);
            warpBuff = AddNewBuff("WarpBuff", Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texChargeBuff"), Color.white, false, false);
        }

        // simple helper method
        internal static BuffDef AddNewBuff(string buffName, Sprite buffIcon, Color buffColor, bool canStack, bool isDebuff)
        {
            BuffDef buffDef = ScriptableObject.CreateInstance<BuffDef>();
            buffDef.name = buffName;
            buffDef.buffColor = buffColor;
            buffDef.canStack = canStack;
            buffDef.isDebuff = isDebuff;
            buffDef.eliteDef = null;
            buffDef.iconSprite = buffIcon;

            buffDefs.Add(buffDef);

            return buffDef;
        }
    }
}