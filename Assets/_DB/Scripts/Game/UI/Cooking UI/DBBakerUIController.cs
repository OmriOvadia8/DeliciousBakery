using DB_Core;
using DG.Tweening;
using System;
using UnityEngine;

namespace DB_Game
{
    public class DBBakerUIController : DBCookingUIBaseController
    {
        [SerializeField] private DBCookingUIManager cookingUIManager;
        private void OnEnable()
        {
          //  AddListener(DBEventNames.OnAutoCookFood, BakerCookingUIStart);
            AddListener(DBEventNames.OnPause, ToggleBakerTweensOnPause);
        }

        private void OnDisable()
        {
          //  RemoveListener(DBEventNames.OnAutoCookFood, BakerCookingUIStart);
            RemoveListener(DBEventNames.OnPause, ToggleBakerTweensOnPause);
        }

        private void Start()
        {
            ResumeBakerCookingAfterPause();
        }

        public void AutoCookFood(int index) // Cooking automatically courotine loop after baker unlocked
        {
            var foodData = DBFoodManager.GetFoodData(index);

            if (foodData.IsAutoOnCooldown)
            {
                return;
            }
            foodData.IsAutoOnCooldown = true;

            BakerCookingUIStart(index);

            InvokeEvent(DBEventNames.OnAutoCookFood, index); // starts the loading bar and timer of cooking
        }

        //private void BakerCookingUIStart(object foodIndex)
        //{
        //    int index = (int)foodIndex;
        //    var foodData = DBFoodManager.GetFoodData(index);
        //    float bakerCookingTime = foodData.BakerCookingTime;

        //    foodData.IsAutoOnCooldown = true;
        //    var bakerSliderBar = cookingUIManager.uiBakerComponents.BakerSliderBar[index];
        //    var bakerTimerText = cookingUIManager.uiBakerComponents.BakerTimerText[index];
        //    var bakerTimeLeftToCook = cookingUIManager.uiBakerComponents.BakerTimeLeftCooking[index];
        //    var remainingBakerTime = cookingUIManager.uiBakerComponents.RemainingBakerTime;

        //    SetCookingUI(ref bakerTimeLeftToCook, index, bakerCookingTime, bakerSliderBar, bakerTimerText, DBTweenTypes.BakerCookingAnim);

        //    var countdownTween = CookingTweenTimer(index, bakerTimeLeftToCook, DBTweenTypes.BakerCookingTimer);
        //    UpdateCookingUIOnTimerTick(index, bakerTimeLeftToCook, countdownTween, foodData, remainingBakerTime, bakerTimerText, CookingType.BakerCooking);
        //    countdownTween.OnComplete(() => OnBakerTimerComplete(index));
        //}

        private void BakerCookingUIStart(int index)
        {
            var foodData = DBFoodManager.GetFoodData(index);
            float bakerCookingTime = foodData.BakerCookingTime;

            foodData.IsAutoOnCooldown = true;
            var bakerSliderBar = cookingUIManager.uiBakerComponents.BakerSliderBar[index];
            var bakerTimerText = cookingUIManager.uiBakerComponents.BakerTimerText[index];
            float bakerTimeLeftToCook = cookingUIManager.uiBakerComponents.BakerTimeLeftCooking[index];
            var remainingBakerTime = cookingUIManager.uiBakerComponents.RemainingBakerTime;

            SetCookingUI(ref bakerTimeLeftToCook, index, bakerCookingTime, bakerSliderBar, bakerTimerText, DBTweenTypes.BakerCookingAnim);

            var countdownTween = CookingTweenTimer(index, bakerTimeLeftToCook, DBTweenTypes.BakerCookingTimer, value => bakerTimeLeftToCook = value);
            UpdateCookingUIOnTimerTick(() => bakerTimeLeftToCook, countdownTween, foodData, remainingBakerTime, bakerTimerText, CookingType.BakerCooking);

            countdownTween.OnComplete(() => OnBakerTimerComplete(index));
        }

        private void BakerCookingTimerAfterPause(int index)
        {
            var foodData = DBFoodManager.GetFoodData(index);
            float bakerCookingTime = foodData.BakerCookingTime;
            var bakerOfflineTime = DBManager.Instance.TimerManager.GetLastOfflineTimeSeconds();

            cookingUIManager.uiBakerComponents.BakerTimeLeftCooking[index] = GetBakerTimeLeftCooking(foodData, bakerCookingTime, bakerOfflineTime);

            foodData.IsAutoOnCooldown = true;
            float bakerTimeLeftToCook = cookingUIManager.uiBakerComponents.BakerTimeLeftCooking[index];
            var bakerSliderBar = cookingUIManager.uiBakerComponents.BakerSliderBar[index];
            var bakerTimerText = cookingUIManager.uiBakerComponents.BakerTimerText[index];
            var remainingBakerTime = cookingUIManager.uiBakerComponents.RemainingBakerTime;

            SetCookingUIAfterPause(foodData, index, bakerCookingTime, bakerTimeLeftToCook,
                                    DBTweenTypes.BakerCookingAnim, bakerSliderBar, bakerTimerText, CookingType.BakerCooking);

            int capturedIndex = index;
            var countdownTween = CookingTweenTimer(capturedIndex, bakerTimeLeftToCook, DBTweenTypes.BakerCookingTimer, x => bakerTimeLeftToCook = x);
            UpdateCookingUIOnTimerTick(() => bakerTimeLeftToCook, countdownTween, foodData, remainingBakerTime, bakerTimerText, CookingType.BakerCooking);
            countdownTween.OnComplete(() => OnBakerTimerComplete(capturedIndex));
        }


        //private void BakerCookingTimerAfterPause(int index)
        //{
        //    var foodData = DBFoodManager.GetFoodData(index);
        //    float bakerCookingTime = foodData.BakerCookingTime;
        //    var bakerOfflineTime = DBManager.Instance.TimerManager.GetLastOfflineTimeSeconds();

        //    cookingUIManager.uiBakerComponents.BakerTimeLeftCooking[index] = GetBakerTimeLeftCooking(foodData, bakerCookingTime, bakerOfflineTime);

        //    foodData.IsAutoOnCooldown = true;
        //    var bakerTimeLeftToCook = cookingUIManager.uiBakerComponents.BakerTimeLeftCooking[index];
        //    var bakerSliderBar = cookingUIManager.uiBakerComponents.BakerSliderBar[index];
        //    var bakerTimerText = cookingUIManager.uiBakerComponents.BakerTimerText[index];
        //    var remainingBakerTime = cookingUIManager.uiBakerComponents.RemainingBakerTime;

        //    SetCookingUIAfterPause(foodData, index, bakerCookingTime, bakerTimeLeftToCook,
        //                            DBTweenTypes.BakerCookingAnim, bakerSliderBar, bakerTimerText, CookingType.BakerCooking);

        //   var countdownTween = CookingTweenTimer(index, bakerTimeLeftToCook, DBTweenTypes.BakerCookingTimer);
        //   UpdateCookingUIOnTimerTick(index, bakerTimeLeftToCook, countdownTween, foodData, remainingBakerTime, bakerTimerText, CookingType.BakerCooking);
        //   countdownTween.OnComplete(() => OnBakerTimerComplete(index));
        //}

        private void OnBakerTimerComplete(int index)
        {
            var foodData = DBFoodManager.GetFoodData(index);
            BakerCookingCompleteReward(foodData, index);
        }

        private void BakerCookingCompleteReward(FoodData foodData, int index)
        {
            foodData.IsAutoOnCooldown = false;
            foodData.RemainingBakerCookingTime = foodData.BakerCookingTime;

            var bakerSliderBar = cookingUIManager.uiBakerComponents.BakerSliderBar[index];
            var bakerTimerText = cookingUIManager.uiBakerComponents.BakerTimerText[index];

            bakerSliderBar.DOValue(bakerSliderBar.minValue, DBCookingUIManager.MIN_VALUE).SetEase(Ease.Linear);

            int profit = (foodData.Profit * DoubleProfitComponent.doubleProfitMultiplier * foodData.CookFoodTimes)
                            + (foodData.CookFoodTimes == 0 ? foodData.Profit : 0);

            InvokeEvent(DBEventNames.AddCurrencyUpdate, profit);
            bakerTimerText.text = DBExtension.GetFormattedTimeSpan((int)foodData.BakerCookingTime);
            InvokeEvent(DBEventNames.MoneyToastOnAutoCook, index);
            AutoCookFood(index);
            InvokeEvent(DBEventNames.BuyButtonsCheck, null);
            DBManager.Instance.SaveManager.Save(DBFoodManager.Foods);
        }

        private float GetBakerTimeLeftCooking(FoodData foodData, float bakerCookingTime, float bakerOfflineTime)
        {
            if (bakerOfflineTime > foodData.RemainingBakerCookingTime)
            {
                float greaterOfflineBakerTime = bakerCookingTime - ((bakerOfflineTime - foodData.RemainingBakerCookingTime) % bakerCookingTime);
                return greaterOfflineBakerTime;
            }
            else
            {
                float LessOfflineBakerTime = foodData.RemainingBakerCookingTime - bakerOfflineTime;
                return LessOfflineBakerTime;
            }
        }

        private void ResumeBakerCookingAfterPause()
        {
            for (int i = 0; i < DBFoodManager.FOOD_COUNT; i++)
            {
                if (DBFoodManager.GetFoodData(i).IsIdleFood)
                {
                    BakerCookingTimerAfterPause(i);
                }
            }
        }

        private void KillBakerTweensOnPause()
        {
            for (int i = 0; i < DBFoodManager.FOOD_COUNT; i++)
            {
                DOTween.Kill(DBTweenTypes.BakerCookingTimer + i);
                DOTween.Kill(DBTweenTypes.BakerCookingAnim + i);
            }
        }

        private void ToggleBakerTweensOnPause(object pauseEvent)
        {
            bool isPaused = (bool)pauseEvent;

            if (isPaused)
            {
                KillBakerTweensOnPause();
            }
            else
            {
                ResumeBakerCookingAfterPause();
            }
        }

    }
}