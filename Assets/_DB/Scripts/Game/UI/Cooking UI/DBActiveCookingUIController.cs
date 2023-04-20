using DB_Core;
using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

namespace DB_Game
{
    public class DBActiveCookingUIController : DBCookingUIBaseController
    {
        [SerializeField] private DBCookingUIManager cookingUIManager;
        [SerializeField] private DBButtonsManager buttonsManager;

        private void OnEnable()
        {
            AddListener(DBEventNames.OnPause, ToggleActiveCookingTweensOnPause);
        }

        private void OnDisable()
        {
            RemoveListener(DBEventNames.OnPause, ToggleActiveCookingTweensOnPause);
        }

        private void Start()
        {
            ResumeActiveCookingAfterPause();
        }

        public void ActiveCookFood(int index)
        {
            var foodData = DBFoodManager.GetFoodData(index);

            if (foodData.IsOnCooldown)
            {
                return;
            }

            foodData.IsOnCooldown = true;

            DBManager.Instance.SaveManager.Save(DBFoodManager.Foods);
            ActiveCookingUIStart(index);
        }

        private void ActiveCookingUIStart(int index) // Starts the tweens of active cooking process
        {           
            var cookFoodParticle = cookingUIManager.uiActiveCookingComponents.ActiveCookingParticles[index];
            cookFoodParticle.Play();

            var foodData = DBFoodManager.GetFoodData(index);
            float cookingTime = foodData.CookingTime;
            foodData.IsOnCooldown = true;

            buttonsManager.CookFoodButtonCheck();
            var cookingSliderBar = cookingUIManager.uiActiveCookingComponents.CookingSliderBar[index];
            var cookingTimerText = cookingUIManager.uiActiveCookingComponents.CookingTimerText[index];
            var cookingTimeLeft = cookingUIManager.uiActiveCookingComponents.TimeLeftToActiveCook[index];
            var remainingCookingtime = cookingUIManager.uiActiveCookingComponents.RemainingActiveCookingTime;

            SetCookingUI(ref cookingTimeLeft, index, cookingTime, cookingSliderBar, cookingTimerText, DBTweenTypes.ActiveCookingAnim);

            var countdownTween = CookingTweenTimer(index, cookingTimeLeft, DBTweenTypes.ActiveCookingTimer, value => cookingTimeLeft = value);
            UpdateCookingUIOnTimerTick(() => cookingTimeLeft, countdownTween, foodData, remainingCookingtime, cookingTimerText, CookingType.ActiveCooking);


            countdownTween.OnComplete(() => OnTimerComplete(index));
        }

        //private void ActiveCookingUIAfterPause(int index) // Starts the tweens of active cooking after the player comes back from pause/offline
        //{
        //    var foodData = DBFoodManager.GetFoodData(index);
        //    float cookingTime = foodData.CookingTime;
        //    var offlineTime = DBManager.Instance.TimerManager.GetLastOfflineTimeSeconds();
        //    cookingUIManager.uiActiveCookingComponents.TimeLeftToActiveCook[index] = foodData.RemainingCookingTime - offlineTime;

        //    var cookingSliderBar = cookingUIManager.uiActiveCookingComponents.CookingSliderBar[index];
        //    var cookingTimerText = cookingUIManager.uiActiveCookingComponents.CookingTimerText[index];
        //    var timeLeftToCook = cookingUIManager.uiActiveCookingComponents.TimeLeftToActiveCook[index];
        //    var remainingCookingTime = cookingUIManager.uiActiveCookingComponents.RemainingActiveCookingTime;

        //    if (cookingUIManager.uiActiveCookingComponents.TimeLeftToActiveCook[index] > 0)
        //    {
        //        foodData.IsOnCooldown = true;
        //        buttonsManager.CookFoodButtonCheck();
        //        SetCookingUIAfterPause(foodData, index, cookingTime, timeLeftToCook, DBTweenTypes.ActiveCookingAnim, cookingSliderBar, cookingTimerText, CookingType.ActiveCooking);

        //       var countdownTween = CookingTweenTimer(index, timeLeftToCook, DBTweenTypes.ActiveCookingTimer);
        //       UpdateCookingUIOnTimerTick(timeLeftToCook, countdownTween, foodData, remainingCookingTime, cookingTimerText, CookingType.ActiveCooking);


        //       countdownTween.OnComplete(() => OnTimerComplete(index));
        //    }
        //    else
        //    {
        //        ResetActiveCookingUI(foodData, cookingSliderBar, cookingTimerText);
        //    }

        //    InvokeEvent(DBEventNames.BuyButtonsCheck, null);
        //}

        private void ActiveCookingUIAfterPause(int index) // Starts the tweens of active cooking after the player comes back from pause/offline
        {
            var foodData = DBFoodManager.GetFoodData(index);
            float cookingTime = foodData.CookingTime;
            var offlineTime = DBManager.Instance.TimerManager.GetLastOfflineTimeSeconds();
            cookingUIManager.uiActiveCookingComponents.TimeLeftToActiveCook[index] = foodData.RemainingCookingTime - offlineTime;

            var cookingSliderBar = cookingUIManager.uiActiveCookingComponents.CookingSliderBar[index];
            var cookingTimerText = cookingUIManager.uiActiveCookingComponents.CookingTimerText[index];
            float timeLeftToCook = cookingUIManager.uiActiveCookingComponents.TimeLeftToActiveCook[index];
            var remainingCookingTime = cookingUIManager.uiActiveCookingComponents.RemainingActiveCookingTime;

            if (timeLeftToCook > 0)
            {
                foodData.IsOnCooldown = true;
                buttonsManager.CookFoodButtonCheck();
                SetCookingUIAfterPause(foodData, index, cookingTime, timeLeftToCook, DBTweenTypes.ActiveCookingAnim, cookingSliderBar, cookingTimerText, CookingType.ActiveCooking);

                int capturedIndex = index;
                var countdownTween = CookingTweenTimer(capturedIndex, timeLeftToCook, DBTweenTypes.ActiveCookingTimer, x => timeLeftToCook = x);
                UpdateCookingUIOnTimerTick(() => timeLeftToCook, countdownTween, foodData, remainingCookingTime, cookingTimerText, CookingType.ActiveCooking);

                countdownTween.OnComplete(() => OnTimerComplete(capturedIndex));
            }
            else
            {
                ResetActiveCookingUI(foodData, cookingSliderBar, cookingTimerText);
            }

            InvokeEvent(DBEventNames.BuyButtonsCheck, null);
        }






        private void OnTimerComplete(int index)
        {
            var foodData = DBFoodManager.GetFoodData(index);
            var cookingSliderBar = cookingUIManager.uiActiveCookingComponents.CookingSliderBar[index];
            var cookingTimerText = cookingUIManager.uiActiveCookingComponents.CookingTimerText[index];

            ResetActiveCookingUI(foodData, cookingSliderBar, cookingTimerText);
            ActiveCookingCompleteReward(foodData, index);
            DBDebug.LogException(foodData.Index + " number " + index + " CD " + foodData.IsOnCooldown);
        }

        private void KillActiveCookingTweensOnPause()
        {
            for (int i = 0; i < DBFoodManager.FOOD_COUNT; i++)
            {
                DOTween.Kill(DBTweenTypes.ActiveCookingAnim + i);
                DOTween.Kill(DBTweenTypes.ActiveCookingTimer + i);
            }
        }

        private void ResumeActiveCookingAfterPause()
        {
            for (int i = 0; i < DBFoodManager.FOOD_COUNT; i++)
            {
                if (DBFoodManager.GetFoodData(i).IsOnCooldown)
                {
                    ActiveCookingUIAfterPause(i);
                }
            }
        }

        private void ActiveCookingCompleteReward(FoodData foodData, int index)
        {
            int profit = foodData.Profit * DoubleProfitComponent.doubleProfitMultiplier;
            InvokeEvent(DBEventNames.AddCurrencyUpdate, profit);
            InvokeEvent(DBEventNames.MoneyToastOnCook, index);
        }

        private void ResetActiveCookingUI(FoodData foodData, Slider cookingSlider, TMP_Text timeText)
        {
            timeText.text = DBExtension.GetFormattedTimeSpan((int)foodData.CookingTime);
            foodData.IsOnCooldown = false;
            cookingSlider.DOValue(cookingSlider.minValue, 0).SetEase(Ease.Linear);
            foodData.RemainingCookingTime = foodData.CookingTime;
            buttonsManager.CookFoodButtonCheck();
            DBManager.Instance.SaveManager.Save(DBFoodManager.Foods);
        }

        private void ToggleActiveCookingTweensOnPause(object pauseEvent)
        {
            bool isPaused = (bool)pauseEvent;

            if (isPaused)
            {
                KillActiveCookingTweensOnPause();
            }
            else
            {
                ResumeActiveCookingAfterPause();
            }
        }

    }
}