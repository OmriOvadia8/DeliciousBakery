using Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;



namespace Game
{
    public class UIManager : HOGLogicMonoBehaviour
    {
        [SerializeField] TMP_Text moneyText;
        [SerializeField] TMP_Text[] foodProfitText;
        [SerializeField] TMP_Text[] foodLevelText;
        [SerializeField] TMP_Text[] upgradeCostText;
        [SerializeField] FoodManager foodManager;
        [SerializeField] Slider[] cookingSliderBar;

        private Tweener tweener;
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

        private void CookingLoadingBarAnimation(object obj)
        {
            float foodCookingTime = foodManager.GetFoodData((int)obj).CookingTime;

            cookingSliderBar[(int)obj].value = minValue;
            tweener = cookingSliderBar[(int)obj].DOValue(maxValue, foodCookingTime);

            tweener.OnComplete(() =>
            {
                cookingSliderBar[(int)obj].value = minValue;
            });
        }
    }
}
  