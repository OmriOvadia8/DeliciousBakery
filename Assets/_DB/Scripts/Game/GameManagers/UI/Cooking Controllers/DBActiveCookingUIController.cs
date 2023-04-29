using DB_Core;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

namespace DB_Game
{
    public class DBActiveCookingUIController : DBCookingUIBaseController
    {
        private void OnEnable() => RegisterEvents();
        
        private void OnDisable() => UnregisterEvents();
        
        private void Start() => ResumeActiveCookingAfterPause();

        public void ActiveCookFood(int index)
        {
            var foodData = GetFoodData(index);

            if (foodData.IsOnCooldown)
            {
                return;
            }

            foodData.IsOnCooldown = true;

            GetFoodData(index);
            ActiveCookingUIStart(index);
        }

        private void ActiveCookingUIStart(int index)
        {
            InvokeEvent(DBEventNames.CookParticles, index);

            var foodData = GetFoodData(index);
            float cookingTime = foodData.CookingTime;
            foodData.IsOnCooldown = true;

            InvokeEvent(DBEventNames.CookFoodButtonCheck, null);
            var cookingSliderBar = CookingUIManager.uiActiveCookingComponents.CookingSliderBar[index];
            var cookingTimerText = CookingUIManager.uiActiveCookingComponents.CookingTimerText[index];
            var cookingTimeLeft = CookingUIManager.uiActiveCookingComponents.TimeLeftToActiveCook[index];
            var remainingCookingtime = CookingUIManager.uiActiveCookingComponents.RemainingActiveCookingTime;

            SetCookingUI(ref cookingTimeLeft, index, cookingTime, cookingSliderBar, cookingTimerText, DBTweenTypes.ActiveCookingAnim);

            var countdownTween = CookingTweenTimer(index, cookingTimeLeft, DBTweenTypes.ActiveCookingTimer, value => cookingTimeLeft = value);
            UpdateCookingUIOnTimerTick(() => cookingTimeLeft, countdownTween, foodData, remainingCookingtime, cookingTimerText, CookingType.ActiveCooking);

            countdownTween.OnComplete(() => OnTimerComplete(index));
        }

        private void ActiveCookingUIAfterPause(int index)
        {
            var foodData = GetFoodData(index);
            var cookingTime = foodData.CookingTime;
            var offlineTime = Manager.TimerManager.GetLastOfflineTimeSeconds();
            CookingUIManager.uiActiveCookingComponents.TimeLeftToActiveCook[index] = foodData.RemainingCookingTime - offlineTime;

            var cookingSliderBar = CookingUIManager.uiActiveCookingComponents.CookingSliderBar[index];
            var cookingTimerText = CookingUIManager.uiActiveCookingComponents.CookingTimerText[index];
            var timeLeftToCook = CookingUIManager.uiActiveCookingComponents.TimeLeftToActiveCook[index];
            var remainingCookingTime = CookingUIManager.uiActiveCookingComponents.RemainingActiveCookingTime;

            if (timeLeftToCook > 0)
            {
                foodData.IsOnCooldown = true;
                InvokeEvent(DBEventNames.CookFoodButtonCheck, null);
                SetCookingUIAfterPause(foodData, index, cookingTime, timeLeftToCook, DBTweenTypes.ActiveCookingAnim, cookingSliderBar, cookingTimerText, CookingType.ActiveCooking);

                int capturedIndex = index;
                var countdownTween = CookingTweenTimer(capturedIndex, timeLeftToCook, DBTweenTypes.ActiveCookingTimer, x => timeLeftToCook = x);
                UpdateCookingUIOnTimerTick(() => timeLeftToCook, countdownTween, foodData, remainingCookingTime, cookingTimerText, CookingType.ActiveCooking);

                countdownTween.OnComplete(() => OnTimerComplete(capturedIndex));
            }
            else
            {
                ResetActiveCookingUI(foodData, cookingSliderBar, cookingTimerText, index);
            }

            InvokeEvent(DBEventNames.BuyButtonsCheck, null);
        }

        private void OnTimerComplete(int index)
        {
            var foodData = GetFoodData(index);
            var cookingSliderBar = CookingUIManager.uiActiveCookingComponents.CookingSliderBar[index];
            var cookingTimerText = CookingUIManager.uiActiveCookingComponents.CookingTimerText[index];

            ResetActiveCookingUI(foodData, cookingSliderBar, cookingTimerText, index);
            ActiveCookingCompleteReward(foodData, index);
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
                if (GetFoodData(i).IsOnCooldown)
                {
                    ActiveCookingUIAfterPause(i);
                }
            }
        }

        private void ActiveCookingCompleteReward(FoodData foodData, int index)
        {
            int profit = foodData.Profit;
            int profitMultiplier = DBDoubleProfitController.DoubleProfitMultiplier;
            int totalProfit = profit * profitMultiplier;
            
            InvokeEvent(DBEventNames.AddCurrencyUpdate, totalProfit);
            InvokeEvent(DBEventNames.MoneyToastOnCook, index);
        }

        private void ResetActiveCookingUI(FoodData foodData, Slider cookingSlider, TMP_Text timeText, int index)
        {
            timeText.text = DBExtension.GetFormattedTimeSpan((int)foodData.CookingTime);
            foodData.IsOnCooldown = false;
            ResetSliderAnimation(cookingSlider);
            foodData.RemainingCookingTime = foodData.CookingTime;
            foodData.FoodCookedCount++;
            InvokeEvent(DBEventNames.CheckCookedAchievement, index);
            InvokeEvent(DBEventNames.CheckTotalCookedAchievement, index);
            InvokeEvent(DBEventNames.CookFoodButtonCheck, null);
            SaveFoodData();
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

        private void RegisterEvents()
        {
            AddListener(DBEventNames.OnPause, ToggleActiveCookingTweensOnPause);
        }

        private void UnregisterEvents()
        {
            RemoveListener(DBEventNames.OnPause, ToggleActiveCookingTweensOnPause);
        }
    }
}