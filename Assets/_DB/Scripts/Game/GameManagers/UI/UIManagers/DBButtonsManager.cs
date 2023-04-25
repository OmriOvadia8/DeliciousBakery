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

        [Header("LearnRecipe")]
        [SerializeField] private Image[] coinIcons;
        [SerializeField] private TMP_Text[] learnRecipeText;

        int CurrentCurrency => currencyManager.currencySaveData.CoinsAmount;

        private void OnEnable() => RegisterEvents();

        private void OnDisable() => UnregisterEvents();

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

        private bool IsAffordable(int cost) => CurrentCurrency >= cost;

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
        }

        private void UnregisterEvents()
        {
            RemoveListener(DBEventNames.BuyButtonsCheck, BuyButtonsCheck);
            RemoveListener(DBEventNames.CookFoodButtonCheck, CookFoodButtonCheck);
            RemoveListener(DBEventNames.CookButtonAlphaOn, CookButtonAlphaOn);
            RemoveListener(DBEventNames.CookButtonAlphaOff, CookButtonAlphaOff);
        }
    }

    [Serializable]
    public class Buttons
    {
        public Button[] CookFoodButtons;
        public Button[] UpgradeButtons;
        public Button[] HireButtons;
        public Button[] LearnRecipeButtons;

        public CanvasGroup[] CookButtonAnimation;
    }
}
