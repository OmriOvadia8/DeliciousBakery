using DB_Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace DB_Game
{
    public class DBButtonsManager : FoodDataAccess
    {
        [Header("Buttons")]
        [SerializeField] private Buttons buttons;

        [Header("Managers")]
        [SerializeField] private DBCurrencyManager currencyManager;
        [SerializeField] DBDeviceSkinDataManager skinStatusManager;

        [Header("LearnRecipe")]
        [SerializeField] private Image[] coinIcons;
        [SerializeField] private TMP_Text[] learnRecipeText;

        private int[] timeWrapPrices = new int[] { DBTimeWrapItem.TWO_HOURS_PRICE, DBTimeWrapItem.FOUR_HOURS_PRICE, DBTimeWrapItem.EIGHT_HOURS_PRICE };
        int CurrentCurrency => currencyManager.currencySaveData.CoinsAmount;
        int CurrentPremiumCurrency => currencyManager.currencySaveData.StarsAmount;

        private void OnEnable() => RegisterEvents();

        private void OnDisable() => UnregisterEvents();

        private void Start()
        {
            UpdateBuyTimeWrapButtonsUI();
            UpdateBuySkinsButtonsUI();
        }

        private void BuyButtonsCheck(object obj = null)
        {
            UpgradeButtonsCheck();
            HireButtonCheck();
            LearnRecipeButtonCheck();
        }

        private void UpgradeButtonsCheck()
        {
            for (int i = 0; i < buttons.UpgradeButtons.Length; i++)
            {
                var upgradeButton = buttons.UpgradeButtons[i];
                int upgradeCost = GetFoodData(i).UpgradeCost;

                bool isAffordable = IsAffordable(upgradeCost);

                upgradeButton.interactable = isAffordable;
            }
        }

        private void LearnRecipeButtonCheck()
        {
            for (int i = 0; i < buttons.LearnRecipeButtons.Length; i++)
            {
                UpdateLearnRecipeButtonUI(i);
            }
        }

        private void HireButtonCheck()
        {
            for (int i = 0; i < buttons.HireButtons.Length; i++)
            {
                int hireCost = GetFoodData(i).HireCost;
                bool isLocked = GetFoodData(i).IsFoodLocked;
                bool canHire = IsAffordable(hireCost) && !isLocked;

                buttons.HireButtons[i].interactable = canHire;
            }
        }

        public void CookFoodButtonCheck(object obj = null)
        {
            for (int i = 0; i < buttons.CookFoodButtons.Length; i++)
            {
                UpdateCookFoodButtonUI(i);
            }
        }

        private void UpdateCookFoodButtonUI(int index)
        {
            var foodData = GetFoodData(index);
            bool isOnCooldown = foodData.IsOnCooldown;

            buttons.CookFoodButtons[index].interactable = !isOnCooldown;

            CookButtonAlphaOn(index);

            if (isOnCooldown)
            {
                CookButtonAlphaOff(index);
            }
        }

        private void UpdateLearnRecipeButtonUI(int index)
        {
            int learnCost = GetFoodData(index).UnlockCost;
            bool isAffordable = IsAffordable(learnCost);

            buttons.LearnRecipeButtons[index].interactable = isAffordable;
            learnRecipeText[index].color = isAffordable ? Color.white : Color.gray;
            coinIcons[index].color = isAffordable ? Color.white : Color.gray;
        }

        private void UpdateBuySkinsButtonsUI(object obj = null)
        {
            bool canBuySkin = IsPremiumAffordable(DBDeviceSkinManager.SKIN_PRICE);
            for (int i = 0; i < buttons.BuySkinOneButtons.Length; i++)
            {
                buttons.BuySkinOneButtons[i].interactable = canBuySkin;
                buttons.BuySkinTwoButtons[i].interactable = canBuySkin;
            }
        }

        private void UpdateBuyTimeWrapButtonsUI(object obj = null)
        {
            for (int i = 0; i < timeWrapPrices.Length; i++)
            {
                buttons.BuyTimeWrapButtons[i].interactable = IsPremiumAffordable(timeWrapPrices[i]);
            }
        }

        private void ShowEquipSkinButtons(object index)
        {
            int deviceIndex = (int)index;
            if (skinStatusManager.SkinUnlockData.Skins[deviceIndex].Color1)
            {
                buttons.EquipSkinOneButtons[deviceIndex].gameObject.SetActive(true);
            }
            if (skinStatusManager.SkinUnlockData.Skins[deviceIndex].Color2)
            {
                buttons.EquipSkinTwoButtons[deviceIndex].gameObject.SetActive(true);
            }
        }

        private bool IsAffordable(int cost) => CurrentCurrency >= cost;

        private bool IsPremiumAffordable(int cost) => CurrentPremiumCurrency >= cost;

        private void CookButtonAlphaOn(object index)
        {
            int foodIndex = (int)index;
            var cookButton = buttons.CookButtonAnimation[foodIndex];
            cookButton.alpha = 1;
        }

        private void CookButtonAlphaOff(object index)
        {
            int foodIndex = (int)index;
            var cookButton = buttons.CookButtonAnimation[foodIndex];
            cookButton.alpha = 0;
        }

        private void RegisterEvents()
        {
            AddListener(DBEventNames.BuyButtonsCheck, BuyButtonsCheck);
            AddListener(DBEventNames.CookFoodButtonCheck, CookFoodButtonCheck);
            AddListener(DBEventNames.CookButtonAlphaOn, CookButtonAlphaOn);
            AddListener(DBEventNames.CookButtonAlphaOff, CookButtonAlphaOff);
            AddListener(DBEventNames.CheckBuySkinButtonUI, UpdateBuySkinsButtonsUI);
            AddListener(DBEventNames.BuySkinButtonVisibility, ShowEquipSkinButtons);
            AddListener(DBEventNames.CheckBuyTimeWrapButtonsUI, UpdateBuyTimeWrapButtonsUI);
        }

        private void UnregisterEvents()
        {
            RemoveListener(DBEventNames.BuyButtonsCheck, BuyButtonsCheck);
            RemoveListener(DBEventNames.CookFoodButtonCheck, CookFoodButtonCheck);
            RemoveListener(DBEventNames.CookButtonAlphaOn, CookButtonAlphaOn);
            RemoveListener(DBEventNames.CookButtonAlphaOff, CookButtonAlphaOff);
            RemoveListener(DBEventNames.CheckBuySkinButtonUI, UpdateBuySkinsButtonsUI);
            RemoveListener(DBEventNames.BuySkinButtonVisibility, ShowEquipSkinButtons);
            RemoveListener(DBEventNames.CheckBuyTimeWrapButtonsUI, UpdateBuyTimeWrapButtonsUI);
        }
    }

    [Serializable]
    public class Buttons
    {
        public Button[] CookFoodButtons;
        public Button[] UpgradeButtons;
        public Button[] HireButtons;
        public Button[] LearnRecipeButtons;
        public Button[] BuySkinOneButtons;
        public Button[] BuySkinTwoButtons;
        public Button[] EquipSkinOneButtons;
        public Button[] EquipSkinTwoButtons;
        public Button[] BuyTimeWrapButtons;

        public CanvasGroup[] CookButtonAnimation;
    }
}
