using DB_Core;
using System;
using UnityEngine;

namespace DB_Game
{
    public class DBDeviceSkinManager : DBLogicMonoBehaviour
    {
        [SerializeField] DBDeviceSkinDataManager skinStatusManager;
        [SerializeField] Device[] devices;
        public const int SKIN_PRICE = 400;
        private int defaultSkin;
        private int firstSkin = 1;
        private int secondSkin = 2;

        private void Start() => ShowCurrentEquippedSkins();

        private void ShowCurrentEquippedSkins()
        {
            int devicesAmount = devices.Length;
            for (int i = 0; i < devicesAmount; i++)
            {
                if (skinStatusManager.SkinUnlockData.Skins[i].Equipped == firstSkin)
                {
                    devices[i].ChangeSkin(firstSkin, false);
                }
                else if (skinStatusManager.SkinUnlockData.Skins[i].Equipped == secondSkin)
                {
                    devices[i].ChangeSkin(secondSkin, false);
                }
                else
                {
                    devices[i].ChangeSkin(defaultSkin, false);
                }

                InvokeEvent(DBEventNames.BuySkinButtonVisibility, i);
            }
        }

        public void UnlockSkinOne(int deviceIndex)
        {
            if (GameLogic.ScoreManager.TryUseScore(ScoreTags.PremiumCurrency, SKIN_PRICE))
            {
                skinStatusManager.SkinUnlockData.Skins[deviceIndex].Color1 = true;
                InvokeUnlockSkinsEvents(deviceIndex);
            }
        }

        public void UnlockSkinTwo(int deviceIndex)
        {
            if (GameLogic.ScoreManager.TryUseScore(ScoreTags.PremiumCurrency, SKIN_PRICE))
            {
                skinStatusManager.SkinUnlockData.Skins[deviceIndex].Color2 = true;
                InvokeUnlockSkinsEvents(deviceIndex);
            }
        }

        private void InvokeUnlockSkinsEvents(int deviceIndex)
        {
            InvokeEvent(DBEventNames.PremCurrencyUpdateUI, null);
            InvokeEvent(DBEventNames.CheckBuySkinButtonUI, null);
            InvokeEvent(DBEventNames.CheckBuyTimeWrapButtonsUI, null);
            InvokeEvent(DBEventNames.BuySkinButtonVisibility, deviceIndex);
            InvokeEvent(DBEventNames.PlaySound, SoundEffectType.ButtonClick);
            skinStatusManager.SaveSkinsUnlockData();
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