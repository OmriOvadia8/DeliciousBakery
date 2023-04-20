using DB_Core;
using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace DB_Game
{
    public class DBCookingUIManager : DBLogicMonoBehaviour
    {
        public const int MIN_VALUE = 0;
        public const int MAX_VALUE = 1;

        [Header("Active Cooking UI Components")]
        public ActiveCookingUIComponents uiActiveCookingComponents;

        [Header("Baker Cooking UI Components")]
        public BakerUIComponents uiBakerComponents;

        private void Start()
        {
            LoadCookingUIInitialization();
        }

        private void LoadCookingUIInitialization()
        {
            for (int i = 0; i < DBFoodManager.FOOD_COUNT; i++)
            {
                InitialCookingUI(i, CookingType.ActiveCooking);
                var foodData = DBFoodManager.GetFoodData(i);

                if (foodData.IsFoodLocked)
                {
                    uiActiveCookingComponents.CookButtonAnimation[i].alpha = 0;

                    if (!foodData.IsIdleFood)
                    {
                        InitialCookingUI(i, CookingType.BakerCooking);
                    }
                }
            }
        }

        private void InitialCookingUI(int i, CookingType cookingType) // Reset food UI
        {
            switch (cookingType)
            {
                case CookingType.ActiveCooking:
                    uiActiveCookingComponents.CookingSliderBar[i].value = MIN_VALUE;
                    uiActiveCookingComponents.CookingTimerText[i].text = DBExtension.GetFormattedTimeSpan((int)(DBFoodManager.GetFoodData(i).CookingTime));
                    break;

                case CookingType.BakerCooking:
                    uiBakerComponents.BakerSliderBar[i].value = MIN_VALUE;
                    uiBakerComponents.BakerTimerText[i].text = DBExtension.GetFormattedTimeSpan((int)(DBFoodManager.GetFoodData(i).BakerCookingTime));
                    break;

                default:
                    throw new ArgumentException("Invalid CookingType value");
            }
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
        public ParticleSystem[] ActiveCookingParticles;

    }

    [Serializable]
    public class BakerUIComponents
    {
        public float[] BakerTimeLeftCooking = new float[DBFoodManager.FOOD_COUNT];
        public TMP_Text[] BakerTimerText;
        public TMP_Text[] BakersCountText;
        public TMP_Text[] CookFoodTimesText;
        public TimeSpan RemainingBakerTime;
        public Slider[] BakerSliderBar;
    }

}
