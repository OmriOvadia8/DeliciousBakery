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
        [SerializeField] FoodManager foodManager;

        [SerializeField] TMP_Text moneyText;

        [SerializeField] TMP_Text[] foodProfitText;
        [SerializeField] TMP_Text[] foodLevelText;
        [SerializeField] TMP_Text[] upgradeCostText;

        [SerializeField] Slider[] cookingSliderBar;
        [SerializeField] TMP_Text[] cookingTimeText;

        private readonly Dictionary<int, Tweener> tweener = new();

        private readonly float minValue = 0f;
        private readonly float maxValue = 1f;


        private void OnEnable()
        {
            AddListener(HOGEventNames.OnCurrencySet, OnMoneyUpdate);
            AddListener(HOGEventNames.OnUpgraded, OnUpgradeUpdate);
            AddListener(HOGEventNames.OnCookFood, CookingLoadingBarAnimation);

            for (int i = 0; i < FoodManager.FOOD_COUNT; i++)
            {
                cookingSliderBar[i].value = minValue;
                // get the cooking time for the food item
                float cookingTime = foodManager.GetFoodData(i).CookingTime;

                // set the cooking time in the timer text
                cookingTimeText[i].text = TimeSpan.FromSeconds(cookingTime).ToString("mm':'ss");
            }
        }

        private void OnDisable()
        {
            RemoveListener(HOGEventNames.OnCurrencySet, OnMoneyUpdate);
            RemoveListener(HOGEventNames.OnUpgraded, OnUpgradeUpdate);
            RemoveListener(HOGEventNames.OnCookFood, CookingLoadingBarAnimation);
        }

        private void OnMoneyUpdate(object obj) // updates player's current money amount text
        {
            int currency = 0;
            if (GameLogic.ScoreManager.TryGetScoreByTag(ScoreTags.GameCurrency, ref currency))
            {
                moneyText.text = currency.ToString();
            }
        }

        private void OnUpgradeUpdate(object obj) // update the foods stats after each upgrade
        {
            int foodLevel = GameLogic.UpgradeManager.GetUpgradeableByID(UpgradeablesTypeID.Food, (int)obj).CurrentLevel;
            int foodProfit = foodManager.GetFoodData((int)obj).Profit;
            int upgradeCost = foodManager.GetFoodData((int)obj).LevelUpCost;

            foodLevelText[(int)obj].text = "Lv. " + foodLevel.ToString();
            foodProfitText[(int)obj].text = foodProfit.ToString();
            upgradeCostText[(int)obj].text = upgradeCost.ToString();
        }

        private void CookingLoadingBarAnimation(object obj) // activates loading bar with DOTween
        {
            float foodCookingTime = foodManager.GetFoodData((int)obj).CookingTime;
            TimeSpan timeLeft = TimeSpan.FromSeconds(foodCookingTime);

            string timeLeftString = string.Format("{0:D2}:{1:D2}", timeLeft.Minutes, timeLeft.Seconds);
            cookingTimeText[(int)obj].text = timeLeftString;

            cookingSliderBar[(int)obj].value = minValue;
            tweener[(int)obj] = cookingSliderBar[(int)obj].DOValue(maxValue, foodCookingTime);

            tweener[(int)obj].OnUpdate(() =>
            {
                timeLeft = TimeSpan.FromSeconds(tweener[(int)obj].Duration() - tweener[(int)obj].Elapsed());
                timeLeftString = string.Format("{0:D2}:{1:D2}", timeLeft.Minutes, timeLeft.Seconds);
                cookingTimeText[(int)obj].text = timeLeftString;
            });

            tweener[(int)obj].OnComplete(() =>
            {
                cookingSliderBar[(int)obj].value = minValue;
                cookingTimeText[(int)obj].text = TimeSpan.FromSeconds(foodCookingTime).ToString("mm':'ss");
            });
        }
    }
}
  