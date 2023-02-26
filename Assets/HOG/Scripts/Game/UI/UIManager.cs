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
        [SerializeField] RectTransform moneyToastPosition;

        [SerializeField] FoodManager foodManager;
        [SerializeField] HOGMoneyHolder moneyHolder;

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

            OnGameLoad();

            for (int i = 0; i < FoodManager.FOOD_COUNT; i++)
            {
                //OnUpgradeUpdate(i);
                cookingSliderBar[i].value = minValue;
                float cookingTime = GetFoodData(i).CookingTime;
                cookingTimeText[i].text = TimeSpan.FromSeconds(cookingTime).ToString("mm':'ss"); // set the cooking time in the timer text
            }
        }

        private void Start()
        {
            Manager.PoolManager.InitPool("MoneyToast", 15, moneyToastPosition);
            
        }

        private void OnDisable()
        {
            RemoveListener(HOGEventNames.OnCurrencySet, OnMoneyUpdate);
            RemoveListener(HOGEventNames.OnUpgraded, OnUpgradeUpdate);
            RemoveListener(HOGEventNames.OnCookFood, CookingLoadingBarAnimation);
            RemoveListener(HOGEventNames.OnCookFood, CookingTimer);
            RemoveListener(HOGEventNames.MoneyToastOnCook, MoneyTextToastAfterCooking);
        }

        private void OnGameLoad()
        {
            moneyHolder.LoadCurrency();
            moneyText.text = $"{moneyHolder.startingCurrency:N0}";
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
            ;
            Vector3 toastPosition = moneyToastPosition.position + Vector3.up * 3;
            moneyToast.transform.position = toastPosition;

            moneyToast.Init(foodProfit);
            Debug.Log(foodProfit);
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