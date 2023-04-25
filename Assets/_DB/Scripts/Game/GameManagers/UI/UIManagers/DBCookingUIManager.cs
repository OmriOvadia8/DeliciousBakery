using DB_Core;
using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace DB_Game
{
    public class DBCookingUIManager : FoodDataAccess
    {
        public const int MIN_VALUE = 0;
        public const int MAX_VALUE = 1;

        [Header("Active Cooking UI Components")]
        public ActiveCookingUIComponents uiActiveCookingComponents;

        [Header("Baker Cooking UI Components")]
        public BakerUIComponents uiBakerComponents;

        private void Start() => LoadCookingUIInitialization();

        private void LoadCookingUIInitialization()
        {
            for (int i = 0; i < DBFoodManager.FOOD_COUNT; i++)
            {
                var foodData = GetFoodData(i);

                if(!foodData.IsOnCooldown)
                {
                    ResetCookingUI(i, CookingType.ActiveCooking);
                }

                if (foodData.IsFoodLocked)
                {
                    ResetCookingUI(i, CookingType.BakerCooking);
                    uiActiveCookingComponents.CookButtonAnimation[i].alpha = 0;
                }
            }
        }

        private void ResetCookingUI(int i, CookingType cookingType) 
        {
            switch (cookingType)
            {
                case CookingType.ActiveCooking:
                    ResetActiveCookingUI(i);
                    break;

                case CookingType.BakerCooking:
                    ResetBakerCookingUI(i);
                    break;

                default:
                    throw new ArgumentException("Invalid CookingType value");
            }
        }

        private void ResetActiveCookingUI(int i)
        {
            var foodData = GetFoodData(i);
            int cookingTime = (int)foodData.CookingTime;
            uiActiveCookingComponents.CookingSliderBar[i].value = MIN_VALUE;
            uiActiveCookingComponents.CookingTimerText[i].text = DBExtension.GetFormattedTimeSpan(cookingTime);
        }

        private void ResetBakerCookingUI(int i)
        {
            var foodData = GetFoodData(i);
            int bakerCookingTime = (int)foodData.BakerCookingTime;
            uiBakerComponents.BakerSliderBar[i].value = MIN_VALUE;
            uiBakerComponents.BakerTimerText[i].text = DBExtension.GetFormattedTimeSpan(bakerCookingTime);
        }
    }

    [Serializable]
    public class ActiveCookingUIComponents
    {
        public float[] TimeLeftToActiveCook = new float[DBFoodManager.FOOD_COUNT];
        public TMP_Text[] CookingTimerText;
        public Slider[] CookingSliderBar;
        public CanvasGroup[] CookButtonAnimation;
        public TimeSpan RemainingActiveCookingTime;
    }

    [Serializable]
    public class BakerUIComponents
    {
        public float[] BakerTimeLeftCooking = new float[DBFoodManager.FOOD_COUNT];
        public TMP_Text[] BakerTimerText;
        public TMP_Text[] BakersCountText;
        public TMP_Text[] CookFoodMultiplierText;
        public TimeSpan RemainingBakerTime;
        public Slider[] BakerSliderBar;
    }
}
