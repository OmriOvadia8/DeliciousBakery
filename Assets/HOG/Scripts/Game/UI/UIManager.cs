using Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;
using System.Collections.Generic;

namespace Game
{
    public class UIManager : HOGLogicMonoBehaviour
    {
        private readonly Dictionary<int, Tweener> foodLoadingBarTweens = new(); // DOTween dictionary - Tween for each cooking food loading bar

        private readonly float minValue = 0f;
        private readonly float maxValue = 1f;

        [SerializeField] HOGTweenMoneyComponent moneyComponent;
        [SerializeField] HOGTweenMoneyComponent SpendMoneyComponent;
        [SerializeField] RectTransform moneyToastPosition;

        [SerializeField] FoodManager foodManager;
        [SerializeField] HOGMoneyHolder moneyHolder;

        [SerializeField] Button[] upgradeButtons;
        [SerializeField] Button[] hireButtons;

        [SerializeField] TMP_Text moneyText;

        [SerializeField] TMP_Text[] foodProfitText;
        [SerializeField] TMP_Text[] foodLevelText;
        [SerializeField] TMP_Text[] upgradeCostText;

        [SerializeField] Slider[] cookingSliderBar;
        [SerializeField] TMP_Text[] cookingTimeText;

        private void OnEnable()
        {
            AddListener(HOGEventNames.OnCurrencySet, OnMoneyUpdate);
            AddListener(HOGEventNames.OnUpgraded, OnUpgradeUpdate);
            AddListener(HOGEventNames.OnCookFood, CookingLoadingBarAnimation);
            AddListener(HOGEventNames.OnCookFood, CookingTimer);
            AddListener(HOGEventNames.MoneyToastOnCook, MoneyTextToastAfterCooking);
            AddListener(HOGEventNames.OnMoneySpentToast, SpendMoneyTextToast);


            OnGameLoad();

            for (int i = 0; i < FoodManager.FOOD_COUNT; i++)
            {
                cookingSliderBar[i].value = minValue;
                float cookingTime = GetFoodData(i).CookingTime;
                cookingTimeText[i].text = TimeSpan.FromSeconds(cookingTime).ToString("mm':'ss"); // set the cooking time in the timer text
            }
        }

        private void Start()
        {
            Manager.PoolManager.InitPool("MoneyToast", 10, moneyToastPosition);
            Manager.PoolManager.InitPool("SpendMoneyToast", 10, moneyToastPosition);
        }

        private void OnDisable()
        {
            RemoveListener(HOGEventNames.OnCurrencySet, OnMoneyUpdate);
            RemoveListener(HOGEventNames.OnUpgraded, OnUpgradeUpdate);
            RemoveListener(HOGEventNames.OnCookFood, CookingLoadingBarAnimation);
            RemoveListener(HOGEventNames.OnCookFood, CookingTimer);
            RemoveListener(HOGEventNames.MoneyToastOnCook, MoneyTextToastAfterCooking);
            RemoveListener(HOGEventNames.OnMoneySpentToast, SpendMoneyTextToast);
        }

        private void OnGameLoad()
        {
            moneyText.text = $"{moneyHolder.currencySaveData.CurrencyAmount:N0}";
        }

        private void OnMoneyUpdate(object obj)
        {
            int currency = 0;
            if (GameLogic.ScoreManager.TryGetScoreByTag(ScoreTags.GameCurrency, ref currency))
            {
                moneyText.text = $"{currency:N0}";
            }
        }

        private void OnUpgradeUpdate(object obj) // update the foods stats text after each upgrade
        {
            int foodLevel = GameLogic.UpgradeManager.GetUpgradeableByID(UpgradeablesTypeID.Food, (int)obj).CurrentLevel;
            int foodProfit = GetFoodData((int)obj).Profit;
            int upgradeCost = GetFoodData((int)obj).UpgradeCost;

            foodLevelText[(int)obj].text = "Lv. " + foodLevel.ToString();
            foodProfitText[(int)obj].text = foodProfit.ToString();
            upgradeCostText[(int)obj].text = upgradeCost.ToString();
        }

        private void CookingLoadingBarAnimation(object obj) // activates loading bar with DOTween
        {
            float foodCookingTime = GetFoodData((int)obj).CookingTime;

            cookingSliderBar[(int)obj].value = minValue;
            foodLoadingBarTweens[(int)obj] = cookingSliderBar[(int)obj].DOValue(maxValue, foodCookingTime);

            foodLoadingBarTweens[(int)obj].OnComplete(() =>
            {
                cookingSliderBar[(int)obj].value = minValue;
                cookingTimeText[(int)obj].text = FormatTimeSpan(TimeSpan.FromSeconds(foodCookingTime));
            });
        }

        private void CookingTimer(object obj) // activates the cooking timer countdown
        {
            float foodCookingTime = GetFoodData((int)obj).CookingTime;
            TimeSpan timeLeft = TimeSpan.FromSeconds(foodCookingTime);

            string timeLeftString = FormatTimeSpan(timeLeft);
            cookingTimeText[(int)obj].text = timeLeftString;

            foodLoadingBarTweens[(int)obj].OnUpdate(() =>
            {
                timeLeft = TimeSpan.FromSeconds(foodLoadingBarTweens[(int)obj].Duration() - foodLoadingBarTweens[(int)obj].Elapsed());
                timeLeftString = FormatTimeSpan(timeLeft);
                cookingTimeText[(int)obj].text = timeLeftString;
            });
        }

        private void MoneyTextToastAfterCooking(object obj) // toasting profit text after cooking
        {
            int foodIndex = (int)obj;
            int foodProfit = GetFoodData(foodIndex).Profit;
            var moneyToast = (HOGTweenMoneyComponent)Manager.PoolManager.GetPoolable(PoolNames.MoneyToast);

            Vector3 toastPosition = moneyToastPosition.position + Vector3.up * 3;
            moneyToast.transform.position = toastPosition;

            moneyToast.Init(foodProfit);
            Debug.Log(foodProfit);
        }

        public void SpendMoneyTextToast(object obj)
        {
            int foodIndex = (int)obj;
            int upgradeCost = GetFoodData(foodIndex).UpgradeCost;
            var moneyToast = (HOGTweenMoneyComponent)Manager.PoolManager.GetPoolable(PoolNames.SpendMoneyToast);

            Vector3 toastPosition = moneyToastPosition.position + Vector3.up * 3;
            moneyToast.transform.position = toastPosition;

            moneyToast.SpendInit(upgradeCost);
        }

        public void UpgradeButtonsCheck()
        {
            for (int i = 0; i < upgradeButtons.Length; i++)
            {
                int upgradeCost = GetFoodData(i).UpgradeCost;
                if (moneyHolder.currencySaveData.CurrencyAmount >= upgradeCost)
                {
                    upgradeButtons[i].interactable = true;
                }
                else
                {
                    upgradeButtons[i].interactable = false;
                }
            }
        }

        public void HireButtonCheck(object obj)
        {
            int buttonIndex = (int)obj;
            int hireCost = GetFoodData(buttonIndex).UpgradeCost;

            foreach (var button in upgradeButtons)
            {
                if (moneyHolder.startingCurrency >= hireCost)
                {
                    button.interactable = true;
                }
                else
                {
                    button.interactable = false;
                }
            }
        }


        private FoodData GetFoodData(int index)
        {
            return foodManager.GetFoodData(index);
        }

        private string FormatTimeSpan(TimeSpan timeSpan)
        {
            return string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        }
    }
}