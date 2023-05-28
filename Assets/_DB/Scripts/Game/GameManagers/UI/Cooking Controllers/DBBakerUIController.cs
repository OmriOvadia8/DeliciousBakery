using DB_Core;
using DG.Tweening;

namespace DB_Game
{
    public class DBBakerUIController : DBCookingUIBaseController
    {
        private void OnEnable() => RegisterEvents();

        private void OnDisable() => UnregisterEvents();

        private void Start() => ResumeBakerCookingAfterPause();
 
        private void BakerCookingUIStart(object foodIndex)
        {
            int index = (int)foodIndex;
            var foodData = GetFoodData(index);

            if (foodData.IsAutoOnCooldown)
            {
                return;
            }

            foodData.IsAutoOnCooldown = true;
            var bakerCookingTime = foodData.BakerCookingTime;
            var bakerSliderBar = CookingUIManager.uiBakerComponents.BakerSliderBar[index];
            var bakerTimerText = CookingUIManager.uiBakerComponents.BakerTimerText[index];
            var bakerTimeLeftToCook = CookingUIManager.uiBakerComponents.BakerTimeLeftCooking[index];
            var remainingBakerTime = CookingUIManager.uiBakerComponents.RemainingBakerTime;

            SetCookingUI(ref bakerTimeLeftToCook, index, bakerCookingTime, bakerSliderBar, bakerTimerText, DBTweenTypes.BakerCookingAnim);

            var countdownTween = CookingTweenTimer(index, bakerTimeLeftToCook, DBTweenTypes.BakerCookingTimer, value => bakerTimeLeftToCook = value);
            UpdateCookingUIOnTimerTick(() => bakerTimeLeftToCook, countdownTween, foodData, remainingBakerTime, bakerTimerText, CookingType.BakerCooking);

            countdownTween.OnComplete(() => OnBakerTimerComplete(index));
        }

        private void BakerCookingTimerAfterPause(int index)
        {
            var foodData = GetFoodData(index);
            var bakerCookingTime = foodData.BakerCookingTime;
            var bakerOfflineTime = Manager.TimerManager.GetLastOfflineTimeSeconds();
            foodData.IsAutoOnCooldown = true;

            CookingUIManager.uiBakerComponents.BakerTimeLeftCooking[index] = GetBakerTimeLeftCooking(foodData, bakerCookingTime, bakerOfflineTime);

            var bakerTimeLeftToCook = CookingUIManager.uiBakerComponents.BakerTimeLeftCooking[index];
            var bakerSliderBar = CookingUIManager.uiBakerComponents.BakerSliderBar[index];
            var bakerTimerText = CookingUIManager.uiBakerComponents.BakerTimerText[index];
            var remainingBakerTime = CookingUIManager.uiBakerComponents.RemainingBakerTime;

            SetCookingUIAfterPause(foodData, index, bakerCookingTime, bakerTimeLeftToCook,
                                    DBTweenTypes.BakerCookingAnim, bakerSliderBar, bakerTimerText, CookingType.BakerCooking);

            int capturedIndex = index;
            var countdownTween = CookingTweenTimer(capturedIndex, bakerTimeLeftToCook, DBTweenTypes.BakerCookingTimer, x => bakerTimeLeftToCook = x);
            UpdateCookingUIOnTimerTick(() => bakerTimeLeftToCook, countdownTween, foodData, remainingBakerTime, bakerTimerText, CookingType.BakerCooking);
            countdownTween.OnComplete(() => OnBakerTimerComplete(capturedIndex));
        }

        private void OnBakerTimerComplete(int index)
        {
            var foodData = GetFoodData(index);
            BakerCookingCompleteReward(foodData, index);
        }

        private void BakerCookingCompleteReward(FoodData foodData, int index)
        {
            foodData.IsAutoOnCooldown = false;
            foodData.RemainingBakerCookingTime = foodData.BakerCookingTime;

            var bakerSliderBar = CookingUIManager.uiBakerComponents.BakerSliderBar[index];
            var bakerTimerText = CookingUIManager.uiBakerComponents.BakerTimerText[index];

            ResetSliderAnimation(bakerSliderBar);

            var profit = CalculateTotalProfit(foodData);

            InvokeEvent(DBEventNames.AddCurrencyUpdate, profit);
            bakerTimerText.text = DBExtension.GetFormattedTimeSpan((int)foodData.BakerCookingTime);
            InvokeEvent(DBEventNames.MoneyToastOnAutoCook, index);
            BakerCookingUIStart(index);
            InvokeEvent(DBEventNames.BuyButtonsCheck, null);
            SaveFoodData();
        }

        private float GetBakerTimeLeftCooking(FoodData foodData, float bakerCookingTime, float bakerOfflineTime)
        {
            return bakerOfflineTime > foodData.RemainingBakerCookingTime
                ? bakerCookingTime - ((bakerOfflineTime - foodData.RemainingBakerCookingTime) % bakerCookingTime)
                : foodData.RemainingBakerCookingTime - bakerOfflineTime;
        }

        private void ResumeBakerCookingAfterPause()
        {
            for (int i = 0; i < DBFoodManager.FOOD_COUNT; i++)
            {
                if (GetFoodData(i).IsBakerUnlocked)
                {
                    BakerCookingTimerAfterPause(i);
                }
            }
        }

        private double CalculateTotalProfit(FoodData foodData)
        {
            double baseProfit = foodData.Profit;
            double cookFoodMultiplier = foodData.CookFoodMultiplier;
            int profitMultiplier = DBDoubleProfitController.DoubleProfitMultiplier;

            double multipliedProfit = baseProfit * profitMultiplier;
            double totalProfit = multipliedProfit * cookFoodMultiplier;

            // Add base profit only when cookFoodTimes is 0
            totalProfit += (cookFoodMultiplier == 0) ? baseProfit : 0;

            return totalProfit;
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

        private void RegisterEvents()
        {
            AddListener(DBEventNames.OnPause, ToggleBakerTweensOnPause);
            AddListener(DBEventNames.StartBakerCooking, BakerCookingUIStart);
        }

        private void UnregisterEvents()
        {
            RemoveListener(DBEventNames.OnPause, ToggleBakerTweensOnPause);
            RemoveListener(DBEventNames.StartBakerCooking, BakerCookingUIStart);
        }
    }
}