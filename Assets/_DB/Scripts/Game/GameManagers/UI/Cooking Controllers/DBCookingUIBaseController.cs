using DB_Core;
using DG.Tweening;
using System;
using UnityEngine.UI;
using TMPro;

namespace DB_Game
{
    /// <summary>
    /// This class serves as the base for the active cooking and baker cooking scripts.
    /// It provides shared methods for managing the cooking user interface and timer functionality.
    /// Both active cooking and baker cooking scripts inherit from this class to utilize the common functionality.
    /// </summary>
    public class DBCookingUIBaseController : FoodDataAccess
    {
        public DBCookingUIManager CookingUIManager;

        protected void SetCookingUI(ref float timeLeft, int index, float cookingTime, Slider cookingSlider, TMP_Text timeText, DBTweenTypes tweenType)
        {
            timeLeft = cookingTime;
            timeText.text = DBExtension.GetFormattedTimeSpan((int)timeLeft);
            cookingSlider.value = cookingSlider.minValue;
            var maxValue = cookingSlider.maxValue;

            AnimateCookingSlider(cookingSlider, maxValue, timeLeft, tweenType, index);
        }

        protected void SetCookingUIAfterPause(FoodData foodData, int index, float cookingTime, float timeLeft,
                                    DBTweenTypes tweenType, Slider cookingSlider, TMP_Text timeText, CookingType cookingType)
        {
            switch (cookingType)
            {
                case CookingType.ActiveCooking:
                    foodData.RemainingCookingTime = timeLeft;
                    break;

                case CookingType.BakerCooking:
                    foodData.RemainingBakerCookingTime = timeLeft;
                    break;

                default: 
                    throw new ArgumentException("Invalid CookingType value");
            }

            timeText.text = DBExtension.GetFormattedTimeSpan((int)timeLeft);
            float fillValue = (cookingTime - timeLeft) / cookingTime;
            cookingSlider.value = fillValue;
            var maxValue = cookingSlider.maxValue;

            AnimateCookingSlider(cookingSlider, maxValue, timeLeft, tweenType, index);
        }

        protected void UpdateCookingUIOnTimerTick(Func<float> getTimeLeft, Tween timerTween, FoodData foodData, TimeSpan remainingTime, TMP_Text timeText, CookingType cookingType)
        {
            int previousTime = (int)getTimeLeft();
            timerTween.OnUpdate(() =>
            {
                float timeLeft = getTimeLeft();
                remainingTime = TimeSpan.FromSeconds(timeLeft);

                if (previousTime != (int)timeLeft)
                {
                    UpdateRemainingCookingTime(cookingType, timeLeft, foodData);
                    previousTime = (int)timeLeft;
                }
                timeText.text = DBExtension.FormatTimeSpan(remainingTime);
            });
        }

        protected Tween CookingTweenTimer(int index, float duration, DBTweenTypes tweenType, Action<float> onUpdate)
        {
            return DOTween.To(() => duration, x => onUpdate(x), DBCookingUIManager.MIN_VALUE, duration).SetEase(Ease.Linear).SetId(tweenType + index);
        }

        private void UpdateRemainingCookingTime(CookingType cookingType, float time, FoodData foodData)
        {
            switch (cookingType)
            {
                case CookingType.ActiveCooking:
                    foodData.RemainingCookingTime = time;
                    break;

                case CookingType.BakerCooking:
                    foodData.RemainingBakerCookingTime = time;
                    break;

                default:
                    throw new ArgumentException("Invalid CookingType value");
            }

            foodDataRepository.SaveFoodData();
        }

        protected void ResetSliderAnimation(Slider slider) =>
            slider.DOValue(slider.minValue, DBCookingUIManager.MIN_VALUE).SetEase(Ease.Linear);
        
        private void AnimateCookingSlider(Slider cookingSlider, float maxValue, float timeLeft, DBTweenTypes tweenType, int index) =>
          cookingSlider.DOValue(maxValue, timeLeft).SetEase(Ease.Linear).SetId(tweenType + index);
    }

    public enum CookingType
    {
        ActiveCooking,
        BakerCooking
    }

    public enum DBTweenTypes
    {
        ActiveCookingAnim,
        ActiveCookingTimer,
        BakerCookingTimer,
        BakerCookingAnim,
        DoubleProfit
    }
}