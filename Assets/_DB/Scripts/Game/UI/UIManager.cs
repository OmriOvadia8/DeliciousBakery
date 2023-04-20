using DB_Core;
using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DB_Game
{
    public class UIManager : DBLogicMonoBehaviour
    {
        private readonly int moneyTextToastAmount = 25;

        [Header("UI Components")]
        [SerializeField] private RectTransform moneyToastPosition;
        [SerializeField] private DBCookingUIManager cookingUIManager;

        [Header("Managers")]
        [SerializeField] private DBCurrencyManager currencyManager;
        [SerializeField] private DBButtonsManager buttonsManager;

        [Header("Texts")]
        [SerializeField] private Texts texts;

        private void OnEnable()
        {
            AddListener(DBEventNames.OnCurrencySet, OnMoneyUpdate);
            AddListener(DBEventNames.OnPremCurrencySet, OnPremMoneyUpdate);
            AddListener(DBEventNames.OnUpgraded, OnUpgradeUpdate);
            AddListener(DBEventNames.MoneyToastOnCook, MoneyTextToastAfterCooking);
            AddListener(DBEventNames.MoneyToastOnAutoCook, MoneyTextToastAfterAutoCooking);
            AddListener(DBEventNames.OnUpgradeMoneySpentToast, SpendUpgradeMoneyTextToast);
            AddListener(DBEventNames.OnHireMoneySpentToast, SpendHireMoneyTextToast);
            AddListener(DBEventNames.OnHired, OnHireUpdate);
            AddListener(DBEventNames.OnLearnRecipe, LearnRecipeTextUpdate);
            AddListener(DBEventNames.OnLearnRecipeSpentToast, SpendLearnRecipeMoneyTextToast);
        }

        private void Start()
        {
            MoneyToastPoolInitialization();
            OnGameLoad();
            
        }

        private void OnDisable()
        {
            RemoveListener(DBEventNames.OnCurrencySet, OnMoneyUpdate);
            RemoveListener(DBEventNames.OnUpgraded, OnUpgradeUpdate);
            RemoveListener(DBEventNames.MoneyToastOnCook, MoneyTextToastAfterCooking);
            RemoveListener(DBEventNames.MoneyToastOnAutoCook, MoneyTextToastAfterAutoCooking);
            RemoveListener(DBEventNames.OnUpgradeMoneySpentToast, SpendUpgradeMoneyTextToast);
            RemoveListener(DBEventNames.OnHireMoneySpentToast, SpendHireMoneyTextToast);
            RemoveListener(DBEventNames.OnHired, OnHireUpdate);
            RemoveListener(DBEventNames.OnLearnRecipe, LearnRecipeTextUpdate);
            RemoveListener(DBEventNames.OnLearnRecipeSpentToast, SpendLearnRecipeMoneyTextToast);
            RemoveListener(DBEventNames.OnPremCurrencySet, OnPremMoneyUpdate);
        }

        private void OnGameLoad()
        {
            UpdateMoneyText();
            UpdateStarText();
        }

        private void UpdateMoneyText()
        {
            var moneyText = texts.MoneyText;
            int currentCurrency = currencyManager.currencySaveData.CurrencyAmount;
            moneyText.text = currentCurrency.ToCurrencyFormat();
        }

        private void UpdateStarText()
        {
            var starText = texts.StarText;
            int currentStars = currencyManager.currencySaveData.PremCurrencyAmount;
            starText.text = currentStars.ToCurrencyFormat();
        }

        private void OnMoneyUpdate(object obj)
        {
            int currency = 0;

            if (GameLogic.ScoreManager.TryGetScoreByTag(ScoreTags.GameCurrency, ref currency))
            {
                var moneyText = texts.MoneyText;
                moneyText.text = currency.ToCurrencyFormat();
            }
        }

        private void OnPremMoneyUpdate(object obj)
        {
            int premCurrency = 0;

            if (GameLogic.ScoreManager.TryGetScoreByTag(ScoreTags.PremiumCurrency, ref premCurrency))
            {
                var starText = texts.StarText;
                starText.text = premCurrency.ToCurrencyFormat();
            }
        }

        private void OnUpgradeUpdate(object obj) // update the foods stats text after each upgrade
        {
            int index = (int)obj;
            int foodLevel = GameLogic.UpgradeManager.GetUpgradeableByID(UpgradeablesTypeID.Food, index).CurrentLevel;
            var foodData = DBFoodManager.GetFoodData(index);
            int foodProfit = foodData.Profit;
            int upgradeCost = foodData.UpgradeCost;

            var foodLevelText = texts.FoodLevelText[index];
            var foodProfitText = texts.FoodProfitText[index];
            var upgradeCostText = texts.UpgradeCostText[index];

            foodLevelText.text = $"Lv. {foodLevel}";
            foodProfitText.text = $"{foodProfit}";
            upgradeCostText.text = $"{upgradeCost}";

            UpdateCurrencyAndButtonCheck();
        }

        private void LearnRecipeTextUpdate(object obj) // update the foods stats text after each upgrade
        {
            int index = (int)obj;
            var foodData = DBFoodManager.GetFoodData(index);

            int learnCost = foodData.UnlockCost;

            var learnRecipeCostText = texts.LearnRecipeCostText[index];
            learnRecipeCostText.text = learnCost.ToString();

            InvokeEvent(DBEventNames.BuyButtonsCheck, null);
        }

        private void OnHireUpdate(object obj) // update stats after hiring
        {
            int index = (int)obj;
            var foodData = DBFoodManager.GetFoodData(index);

            int hireCost = foodData.HireCost;
            int cookFoodTimes = foodData.CookFoodTimes;
            int bakersCount = foodData.BakersCount;

            var bakersCountText = cookingUIManager.uiBakerComponents.BakersCountText[index];
            var cookFoodTimesText = cookingUIManager.uiBakerComponents.CookFoodTimesText[index];
            var hireCostText = texts.HireCostText[index];

            bakersCountText.text = $"{bakersCount}x";
            cookFoodTimesText.text = $"{cookFoodTimes}x";
            hireCostText.text = $"{hireCost}";

            UpdateCurrencyAndButtonCheck();
        }

        private void MoneyTextToastAfterCooking(object obj) // toasting profit text after cooking (ACTIVE cooking)
        {
            buttonsManager.CookFoodButtonCheck();
            int index = (int)obj;
            int foodProfit = DBFoodManager.GetFoodData(index).Profit;
            int totalFoodProfit = foodProfit * DoubleProfitComponent.doubleProfitMultiplier;
            MoneyToasting(totalFoodProfit, PoolNames.MoneyToast);
            InvokeEvent(DBEventNames.BuyButtonsCheck, null);
        }

        private void MoneyTextToastAfterAutoCooking(object obj) // toasting profit text after cooking (PASSIVE cooking)
        {
            int index = (int)obj;
            int foodProfit = DBFoodManager.GetFoodData(index).Profit;
            int cookFoodQuantity = DBFoodManager.GetFoodData(index).CookFoodTimes;
            buttonsManager.CookFoodButtonCheck();
            int totalFoodProfit = foodProfit * cookFoodQuantity * DoubleProfitComponent.doubleProfitMultiplier;
            MoneyToasting(totalFoodProfit, PoolNames.MoneyToast);
            InvokeEvent(DBEventNames.BuyButtonsCheck, null);
        }

        private void SpendUpgradeMoneyTextToast(object obj) // toasting cost text after upgrading
        {
            int index = (int)obj;
            int upgradeCost = DBFoodManager.GetFoodData(index).UpgradeCost;
            MoneyToasting(upgradeCost, PoolNames.SpendMoneyToast);
        }

        private void SpendLearnRecipeMoneyTextToast(object obj) // toasting cost text after learning
        {
            int index = (int)obj;
            int learnCost = DBFoodManager.GetFoodData(index).UnlockCost;
            InvokeEvent(DBEventNames.CookButtonAlphaOn, index);
            MoneyToasting(learnCost, PoolNames.SpendMoneyToast);
            UpdateCurrencyAndButtonCheck();
        }

        private void SpendHireMoneyTextToast(object obj) // // toasting cost text after hiring
        {
            int index = (int)obj;
            int hireCost = DBFoodManager.GetFoodData(index).HireCost;
            MoneyToasting(hireCost, PoolNames.SpendMoneyToast);
        }

    
  
        private void MoneyToastPoolInitialization()
        {
            Manager.PoolManager.InitPool("MoneyToast", moneyTextToastAmount, moneyToastPosition);
            Manager.PoolManager.InitPool("SpendMoneyToast", moneyTextToastAmount, moneyToastPosition);
        }

        private void UpdateCurrencyAndButtonCheck()
        {
            currencyManager.UpdateCurrency(currencyManager.startingCurrency);
            InvokeEvent(DBEventNames.BuyButtonsCheck, null);
        }

        private void MoneyToasting(int moneyAmount, PoolNames poolName)
        {
            var moneyToast = (DBTweenMoneyComponent)Manager.PoolManager.GetPoolable(poolName);
            Vector3 toastPosition = moneyToastPosition.position;
            moneyToast.transform.position = toastPosition;

            switch (poolName)
            {
                case PoolNames.MoneyToast:
                    moneyToast.Init(moneyAmount);
                    break;

                case PoolNames.SpendMoneyToast:
                    moneyToast.SpendInit(moneyAmount);
                    break;

                default:
                    throw new ArgumentException("Invalid PoolName value");
            }
        }
    }

    public enum DBTweenTypes
    {
        ActiveCookingAnim,
        ActiveCookingTimer,
        BakerCookingTimer,
        BakerCookingAnim,
        DoubleProfit
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