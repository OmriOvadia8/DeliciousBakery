using DB_Core;
using UnityEngine;

namespace DB_Game
{
    public class DBSkinUnlockController : DBLogicMonoBehaviour
    {
        [SerializeField] DBDeviceSkinStatusDataManager skinStatusManager;
        public const int SKIN_PRICE = 100;

        public void UnlockSkinOne(int deviceIndex)
        {
            if (GameLogic.ScoreManager.TryUseScore(ScoreTags.PremiumCurrency, SKIN_PRICE))
            {
                skinStatusManager.SkinUnlockData.Skins[deviceIndex].Color1 = true;

                InvokeEvent(DBEventNames.PremCurrencyUpdateUI, null);
                InvokeEvent(DBEventNames.CheckBuySkinButtonUI, null);
                skinStatusManager.SaveSkinsUnlockData();
            }
        }

        public void UnlockSkinTwo(int deviceIndex)
        {
            if (GameLogic.ScoreManager.TryUseScore(ScoreTags.PremiumCurrency, SKIN_PRICE))
            {
                skinStatusManager.SkinUnlockData.Skins[deviceIndex].Color2 = true;
                InvokeEvent(DBEventNames.PremCurrencyUpdateUI, null);
                InvokeEvent(DBEventNames.CheckBuySkinButtonUI, null);
                skinStatusManager.SaveSkinsUnlockData();
            }
        }

        public void CheckSkinStatus(int deviceIndex)
        {
            InvokeEvent(DBEventNames.CheckBuySkinButtonUI, null);
            DBDebug.Log(skinStatusManager.SkinUnlockData.Skins[deviceIndex].Color1);
            DBDebug.Log(skinStatusManager.SkinUnlockData.Skins[deviceIndex].Color2);
        }
    }

    public enum KitchenDevices
    {
        Stove1 = 0,
        Stove2 = 1,
        IcecreamStand = 2,
        Fridge = 3,
        Mixer = 4,
        Blender = 5,
        ChocoMachine = 6,
        CoffeeMachine = 7
    }
}