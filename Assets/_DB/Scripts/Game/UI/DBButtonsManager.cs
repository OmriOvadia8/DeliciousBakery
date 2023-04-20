using DB_Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace DB_Game
{
    public class DBButtonsManager : DBLogicMonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] public Buttons buttons;

        [Header("Managers")]
        [SerializeField] private DBCurrencyManager currencyManager;

        [Header("LearnRecipe")]
        [SerializeField] private Image[] coinIcons;
        [SerializeField] private TMP_Text[] learnRecipeText;

        private void OnEnable()
        {
            AddListener(DBEventNames.BuyButtonsCheck, BuyButtonsCheck);
            //AddListener(DBEventNames.CookFoodButtonCheck, CookFoodButtonCheck);
            AddListener(DBEventNames.CookButtonAlphaOn, CookButtonAlphaOn);
            AddListener(DBEventNames.CookButtonAlphaOff, CookButtonAlphaOff);
        }

        private void OnDisable()
        {
            RemoveListener(DBEventNames.BuyButtonsCheck, BuyButtonsCheck);
            //RemoveListener(DBEventNames.CookFoodButtonCheck, CookFoodButtonCheck);
            RemoveListener(DBEventNames.CookButtonAlphaOn, CookButtonAlphaOn);
            RemoveListener(DBEventNames.CookButtonAlphaOff, CookButtonAlphaOff);
        }


        private void BuyButtonsCheck(object obj = null) // checks if all buying buttons are interactable if player has enough money
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
                int upgradeCost = DBFoodManager.GetFoodData(i).UpgradeCost;
                bool isAffordable = currencyManager.currencySaveData.CurrencyAmount >= upgradeCost;

                upgradeButton.interactable = isAffordable;
            }
        }

        private void LearnRecipeButtonCheck()
        {
            var learnRecipeButtons = buttons.LearnRecipeButtons;
            var currentCurrency = currencyManager.currencySaveData.CurrencyAmount;

            for (int i = 0; i < learnRecipeButtons.Length; i++)
            {
                int learnCost = DBFoodManager.GetFoodData(i).UnlockCost;
                if (currentCurrency >= learnCost)
                {
                    learnRecipeButtons[i].interactable = true;
                    learnRecipeText[i].color = Color.white;
                    coinIcons[i].color = Color.white;
                }
                else
                {
                    learnRecipeButtons[i].interactable = false;
                    learnRecipeText[i].color = Color.gray;
                    coinIcons[i].color = Color.gray;
                }
            }
        }

        private void HireButtonCheck()
        {
            var currentCurrency = currencyManager.currencySaveData.CurrencyAmount;
            for (int i = 0; i < buttons.HireButtons.Length; i++)
            {
                int hireCost = DBFoodManager.GetFoodData(i).HireCost;
                bool isLocked = DBFoodManager.GetFoodData(i).IsFoodLocked;
                buttons.HireButtons[i].interactable = currentCurrency >= hireCost && !isLocked;
            }
        }

        public void CookFoodButtonCheck() // checks if food is already being cooked so player must wait to cook again that food
        {
            for (int i = 0; i < buttons.CookFoodButtons.Length; i++)
            {
                var foodData = DBFoodManager.GetFoodData(i);
                bool isOnCooldown = foodData.IsOnCooldown;

                if (!isOnCooldown)
                {
                    buttons.CookFoodButtons[i].interactable = true;
                    CookButtonAlphaOn(i);
                }
                else
                {
                    buttons.CookFoodButtons[i].interactable = false;
                    CookButtonAlphaOff(i);
                }
            }
        }


        private void CookButtonAlphaOn(object index)
        {
            int foodIndex = (int)index;
            buttons.CookButtonAnimation[foodIndex].alpha = 1;
        }

        private void CookButtonAlphaOff(object index)
        {
            int foodIndex = (int)index;
            buttons.CookButtonAnimation[foodIndex].alpha = 0;
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
