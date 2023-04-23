using DB_Core;
using System;
using TMPro;
using UnityEngine;

namespace DB_Game
{
    public class DBTextsManager : FoodDataAccess
    {
        [Header("Managers")]
        [SerializeField] private DBCurrencyManager currencyManager;
        [SerializeField] private DBToastingManager toastingManager;
        [SerializeField] private DBCookingUIManager cookingUIManager;

        [Header("Texts")]
        [SerializeField] private Texts texts;

        private void OnEnable() => AddTextsEvents();
   
        private void OnDisable() => RemoveTextsEvents();

        private void Start() => UpdateCurrencyAmountText();

        #region Currency Texts

        private void UpdateCurrencyAmountText()
        {
            UpdateCoinsAmountText();
            UpdateStarsAmountText();
        }

        private void UpdateCoinsAmountText()
        {
            var moneyText = texts.MoneyText;
            int currentCurrency = currencyManager.currencySaveData.CoinsAmount;
            moneyText.text = currentCurrency.ToCurrencyFormat();
        }

        private void UpdateStarsAmountText()
        {
            var starText = texts.StarText;
            int currentStars = currencyManager.currencySaveData.StarsAmount;
            starText.text = currentStars.ToCurrencyFormat();
        }

        private void OnCoinsAmountUpdate(object obj)
        {
            int currency = 0;

            if (GameLogic.ScoreManager.TryGetScoreByTag(ScoreTags.GameCurrency, ref currency))
            {
                var moneyText = texts.MoneyText;
                moneyText.text = currency.ToCurrencyFormat();
            }
        }

        private void OnStarsAmountUpdate(object obj)
        {
            int premCurrency = 0;

            if (GameLogic.ScoreManager.TryGetScoreByTag(ScoreTags.PremiumCurrency, ref premCurrency))
            {
                var starText = texts.StarText;
                starText.text = premCurrency.ToCurrencyFormat();
            }
        }

        #endregion

        #region Texts Updates On Actions

        private void OnUpgradeTextUpdate(object obj)
        {
            int index = (int)obj;
            int foodLevel = GameLogic.UpgradeManager.GetUpgradeableByID(UpgradeablesTypeID.Food, index).CurrentLevel;
            var foodData = foodDataRepository.GetFoodData(index);
            int foodProfit = foodData.Profit;
            int upgradeCost = foodData.UpgradeCost;

            UpdateUpgradeTexts(index, foodLevel, foodProfit, upgradeCost);
            UpdateCurrencyAndButtonCheck();
        }

        private void LearnRecipeTextUpdate(object obj) 
        {
            int index = (int)obj;
            var foodData = foodDataRepository.GetFoodData(index);
            int learnCost = foodData.UnlockCost;
            var learnRecipeCostText = texts.LearnRecipeCostText[index];

            learnRecipeCostText.text = learnCost.ToString();

            InvokeEvent(DBEventNames.BuyButtonsCheck, null);
        }

        private void OnHireTextUpdate(object obj)
        {
            int index = (int)obj;
            var foodData = foodDataRepository.GetFoodData(index);

            int hireCost = foodData.HireCost;
            int cookFoodTimes = foodData.CookFoodTimes;
            int bakersCount = foodData.BakersCount;

            UpdateHireTexts(index, bakersCount, cookFoodTimes, hireCost);
            UpdateCurrencyAndButtonCheck();
        }

        private void UpdateCurrencyAndButtonCheck()
        {
            currencyManager.CoinsEarn(currencyManager.initialCoinsAmount);
            InvokeEvent(DBEventNames.BuyButtonsCheck, null);
        }

        private void UpdateHireTexts(int index, int bakersCount, int cookFoodTimes, int hireCost)
        {
            var bakersCountText = cookingUIManager.uiBakerComponents.BakersCountText[index];
            var cookFoodTimesText = cookingUIManager.uiBakerComponents.CookFoodTimesText[index];
            var hireCostText = texts.HireCostText[index];

            bakersCountText.text = $"{bakersCount}x";
            cookFoodTimesText.text = $"{cookFoodTimes}x";
            hireCostText.text = $"{hireCost}";
        }

        private void UpdateUpgradeTexts(int index, int foodLevel, int foodProfit, int upgradeCost)
        {
            var foodLevelText = texts.FoodLevelText[index];
            var foodProfitText = texts.FoodProfitText[index];
            var upgradeCostText = texts.UpgradeCostText[index];

            foodLevelText.text = $"Lv. {foodLevel}";
            foodProfitText.text = $"{foodProfit}";
            upgradeCostText.text = $"{upgradeCost}";
        }

        #endregion

        #region Text Toastings

        private void MoneyTextToastAfterActiveCooking(object obj) 
        {
            InvokeEvent(DBEventNames.CookFoodButtonCheck, null);
            int index = (int)obj;
            int foodProfit = foodDataRepository.GetFoodData(index).Profit;
            int totalFoodProfit = foodProfit * DoubleProfitComponent.doubleProfitMultiplier;
            toastingManager.MoneyToasting(totalFoodProfit, PoolNames.MoneyToast);
            InvokeEvent(DBEventNames.BuyButtonsCheck, null);
        }

        private void MoneyTextToastAfterBakerCooking(object obj) 
        {
            int index = (int)obj;
            int foodProfit = foodDataRepository.GetFoodData(index).Profit;
            int cookFoodQuantity = foodDataRepository.GetFoodData(index).CookFoodTimes;
            InvokeEvent(DBEventNames.CookFoodButtonCheck, null);
            int totalFoodProfit = foodProfit * cookFoodQuantity * DoubleProfitComponent.doubleProfitMultiplier;
            toastingManager.MoneyToasting(totalFoodProfit, PoolNames.MoneyToast);
            InvokeEvent(DBEventNames.BuyButtonsCheck, null);
        }

        private void SpendUpgradeMoneyTextToast(object obj) 
        {
            int index = (int)obj;
            int upgradeCost = foodDataRepository.GetFoodData(index).UpgradeCost;
            toastingManager.MoneyToasting(upgradeCost, PoolNames.SpendMoneyToast);
        }

        private void SpendLearnRecipeMoneyTextToast(object obj) 
        {
            int index = (int)obj;
            int learnCost = foodDataRepository.GetFoodData(index).UnlockCost;
            InvokeEvent(DBEventNames.CookButtonAlphaOn, index);
            toastingManager.MoneyToasting(learnCost, PoolNames.SpendMoneyToast);
            UpdateCurrencyAndButtonCheck();
        }

        private void SpendHireMoneyTextToast(object obj) 
        {
            int index = (int)obj;
            int hireCost = foodDataRepository.GetFoodData(index).HireCost;
            toastingManager.MoneyToasting(hireCost, PoolNames.SpendMoneyToast);
        }

        #endregion

        #region Events Register/Unregister

        private void AddTextsEvents()
        {
            AddListener(DBEventNames.OnCurrencySet, OnCoinsAmountUpdate);
            AddListener(DBEventNames.OnPremCurrencySet, OnStarsAmountUpdate);
            AddListener(DBEventNames.OnUpgraded, OnUpgradeTextUpdate);
            AddListener(DBEventNames.MoneyToastOnCook, MoneyTextToastAfterActiveCooking);
            AddListener(DBEventNames.MoneyToastOnAutoCook, MoneyTextToastAfterBakerCooking);
            AddListener(DBEventNames.OnUpgradeMoneySpentToast, SpendUpgradeMoneyTextToast);
            AddListener(DBEventNames.OnHireMoneySpentToast, SpendHireMoneyTextToast);
            AddListener(DBEventNames.OnHired, OnHireTextUpdate);
            AddListener(DBEventNames.OnLearnRecipe, LearnRecipeTextUpdate);
            AddListener(DBEventNames.OnLearnRecipeSpentToast, SpendLearnRecipeMoneyTextToast);
        }

        private void RemoveTextsEvents()
        {
            RemoveListener(DBEventNames.OnCurrencySet, OnCoinsAmountUpdate);
            RemoveListener(DBEventNames.OnUpgraded, OnUpgradeTextUpdate);
            RemoveListener(DBEventNames.MoneyToastOnCook, MoneyTextToastAfterActiveCooking);
            RemoveListener(DBEventNames.MoneyToastOnAutoCook, MoneyTextToastAfterBakerCooking);
            RemoveListener(DBEventNames.OnUpgradeMoneySpentToast, SpendUpgradeMoneyTextToast);
            RemoveListener(DBEventNames.OnHireMoneySpentToast, SpendHireMoneyTextToast);
            RemoveListener(DBEventNames.OnHired, OnHireTextUpdate);
            RemoveListener(DBEventNames.OnLearnRecipe, LearnRecipeTextUpdate);
            RemoveListener(DBEventNames.OnLearnRecipeSpentToast, SpendLearnRecipeMoneyTextToast);
            RemoveListener(DBEventNames.OnPremCurrencySet, OnStarsAmountUpdate);
        }

        #endregion
    }

    [Serializable]

    public class Texts
    {
        public TMP_Text MoneyText;
        public TMP_Text StarText;

        public TMP_Text[] FoodProfitText;
        public TMP_Text[] FoodLevelText;
        public TMP_Text[] UpgradeCostText;
        public TMP_Text[] HireCostText;
        public TMP_Text[] LearnRecipeCostText;
    }
}