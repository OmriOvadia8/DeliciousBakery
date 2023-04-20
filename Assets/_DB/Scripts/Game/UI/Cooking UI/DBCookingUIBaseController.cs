using DB_Core;
using DG.Tweening;
using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

namespace DB_Game
{
    public class DBCookingUIBaseController : DBLogicMonoBehaviour
    {
        protected void SetCookingUI(ref float timeLeft, int index, float cookingTime, Slider cookingSlider, TMP_Text timeText, DBTweenTypes tweenType)
        {
            timeLeft = cookingTime;
            timeText.text = DBExtension.GetFormattedTimeSpan((int)timeLeft);
            cookingSlider.value = cookingSlider.minValue;
            cookingSlider.DOValue(1, timeLeft).SetEase(Ease.Linear).SetId(tweenType + index);
        }

        protected void SetCookingUIAfterPause(FoodData foodData, int index, float cookingTime, float timeLeft,
                                    DBTweenTypes tweenType, Slider cookingSlider, TMP_Text timeText, CookingType cookingType)
        {
            switch (cookingType)
            {
                case CookingType.ActiveCooking: foodData.RemainingCookingTime = timeLeft; break;
                case CookingType.BakerCooking: foodData.RemainingBakerCookingTime = timeLeft; break;
                default: throw new ArgumentException("Invalid CookingType value");
            }

            timeText.text = DBExtension.GetFormattedTimeSpan((int)timeLeft);
            float fillValue = (cookingTime - timeLeft) / cookingTime;
            cookingSlider.value = fillValue;
            cookingSlider.DOValue(1, timeLeft).SetEase(Ease.Linear).SetId(tweenType + index);
        }


        //protected void UpdateCookingUIOnTimerTick(float timeLeft, Tween timerTween, FoodData foodData, TimeSpan remainingTime, TMP_Text timeText, CookingType cookingType)
        //{
        //    int previousTime = (int)timeLeft;
        //    timerTween.OnUpdate(() =>
        //    {
        //        remainingTime = TimeSpan.FromSeconds(timeLeft);
        //        DBDebug.LogException(remainingTime);
        //        if (previousTime != (int)timeLeft)
        //        {
        //            UpdateRemainingCookingTime(cookingType, timeLeft, foodData);
        //            previousTime = (int)timeLeft;
        //            DBDebug.LogException(remainingTime);
        //        }
        //        timeText.text = DBExtension.FormatTimeSpan(remainingTime);
        //    });
        //}

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
            DBManager.Instance.SaveManager.Save(DBFoodManager.Foods);
        }

        protected Tween CookingTweenTimer(int index, float duration, DBTweenTypes tweenType, Action<float> onUpdate)
        {
            return DOTween.To(() => duration, x => onUpdate(x), 0, duration).SetEase(Ease.Linear).SetId(tweenType + index);
        }
    }

    public enum CookingType
    {
        ActiveCooking,
        BakerCooking
    }
}