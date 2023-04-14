using DB_Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;

namespace DB_Game
{
    public class UIManager : DBLogicMonoBehaviour
    {
        private readonly int minValue = 0;
        private readonly int maxValue = 1;
        private readonly int moneyTextToastAmount = 25;

        private int offlineTime;
        private int bakerOfflineTime;
        private float[] timeLeftToCook = new float[DBFoodManager.FOOD_COUNT];
        private float[] bakerTimeLeftCooking = new float[DBFoodManager.FOOD_COUNT];

        TimeSpan remainingCookingTime;
        TimeSpan remainingBakerTime;

        [Header("Components")]
        [SerializeField] RectTransform moneyToastPosition;

        [Header("Managers")]
        [SerializeField] DBFoodManager foodManager;
        [SerializeField] DBCurrencyManager currencyManager;
        [SerializeField] DBCookingManager cookingManager;

        [Header("Buttons")]
        [SerializeField] Button[] cookFoodButtons;
        [SerializeField] Button[] upgradeButtons;
        [SerializeField] Button[] hireButtons;
        [SerializeField] Button[] learnRecipeButtons;
        [SerializeField] CanvasGroup[] cookButtonAnimation;

        [Header("Texts")]
        [SerializeField] TMP_Text moneyText;
        [SerializeField] TMP_Text starText;
        [SerializeField] TMP_Text[] foodProfitText;
        [SerializeField] TMP_Text[] foodLevelText;
        [SerializeField] TMP_Text[] upgradeCostText;
        [SerializeField] TMP_Text[] cookFoodTimesText;
        [SerializeField] TMP_Text[] hireCostText;
        [SerializeField] TMP_Text[] bakersCountText;
        [SerializeField] TMP_Text[] learnRecipeCostText;
        [SerializeField] TMP_Text[] learnRecipeText;
        [SerializeField] Image[] coinIcons;

        [Header("Sliders")]
        [SerializeField] Slider[] cookingSliderBar;
        [SerializeField] TMP_Text[] cookingTimeText;
        [SerializeField] Slider[] bakerSliderBar;
        [SerializeField] TMP_Text[] bakerTimeText;

        private void OnEnable()
        {
            AddListener(DBEventNames.OnCurrencySet, OnMoneyUpdate);
            AddListener(DBEventNames.OnPremCurrencySet, OnPremMoneyUpdate);
            AddListener(DBEventNames.OnUpgraded, OnUpgradeUpdate);
            AddListener(DBEventNames.OnCookFood, ActiveCookingUIStart);
            AddListener(DBEventNames.OnPause, CookingPauseCheck);
            AddListener(DBEventNames.MoneyToastOnCook, MoneyTextToastAfterCooking);
            AddListener(DBEventNames.MoneyToastOnAutoCook, MoneyTextToastAfterAutoCooking);
            AddListener(DBEventNames.OnUpgradeMoneySpentToast, SpendUpgradeMoneyTextToast);
            AddListener(DBEventNames.OnHireMoneySpentToast, SpendHireMoneyTextToast);
            AddListener(DBEventNames.OnHired, OnHireUpdate);
            AddListener(DBEventNames.OnAutoCookFood, BakerStartTimer);
            AddListener(DBEventNames.OnLearnRecipe, LearnRecipeTextUpdate);
            AddListener(DBEventNames.OnLearnRecipeSpentToast, SpendLearnRecipeMoneyTextToast);
        }

        private void Start()
        {
            MoneyToastPoolInitialization();
            OnGameLoad();
            CookingPauseCheck(false);
        }

        private void OnDisable()
        {
            RemoveListener(DBEventNames.OnCurrencySet, OnMoneyUpdate);
            RemoveListener(DBEventNames.OnUpgraded, OnUpgradeUpdate);
            RemoveListener(DBEventNames.OnCookFood, ActiveCookingUIStart);
            RemoveListener(DBEventNames.OnPause, CookingPauseCheck);
            RemoveListener(DBEventNames.MoneyToastOnCook, MoneyTextToastAfterCooking);
            RemoveListener(DBEventNames.MoneyToastOnAutoCook, MoneyTextToastAfterAutoCooking);
            RemoveListener(DBEventNames.OnUpgradeMoneySpentToast, SpendUpgradeMoneyTextToast);
            RemoveListener(DBEventNames.OnHireMoneySpentToast, SpendHireMoneyTextToast);
            RemoveListener(DBEventNames.OnHired, OnHireUpdate);
            RemoveListener(DBEventNames.OnAutoCookFood, BakerStartTimer);
            RemoveListener(DBEventNames.OnLearnRecipe, LearnRecipeTextUpdate);
            RemoveListener(DBEventNames.OnLearnRecipeSpentToast, SpendLearnRecipeMoneyTextToast);
            RemoveListener(DBEventNames.OnPremCurrencySet, OnPremMoneyUpdate);
        }

        private void OnGameLoad()
        {
            moneyText.text = $"{currencyManager.currencySaveData.CurrencyAmount:N0}";
            starText.text = $"{currencyManager.currencySaveData.PremCurrencyAmount:N0}";

            for (int i = 0; i < DBFoodManager.FOOD_COUNT; i++)
            {
                InitialCookingUI(i, CookingType.ActiveCooking);

                if (DBFoodManager.GetFoodData(i).IsIdleFood == false)
                {
                    InitialCookingUI(i, CookingType.BakerCooking);
                }

                if (DBFoodManager.GetFoodData(i).IsFoodLocked == true)
                {
                    cookButtonAnimation[i].alpha = 0;
                }
            }
        }

        private void OnMoneyUpdate(object obj)
        {
            int currency = 0;
            if (GameLogic.ScoreManager.TryGetScoreByTag(ScoreTags.GameCurrency, ref currency))
            {
                moneyText.text = $"{currency:N0}";
            }
        }

        private void OnPremMoneyUpdate(object obj)
        {
            int premCurrency = 0;
            if (GameLogic.ScoreManager.TryGetScoreByTag(ScoreTags.PremiumCurrency, ref premCurrency))
            {
                starText.text = $"{premCurrency:N0}";
            }
        }

        private void OnUpgradeUpdate(object obj) // update the foods stats text after each upgrade
        {
            int index = (int)obj;
            int foodLevel = GameLogic.UpgradeManager.GetUpgradeableByID(UpgradeablesTypeID.Food, index).CurrentLevel;
            int foodProfit = DBFoodManager.GetFoodData(index).Profit;
            int upgradeCost =   DBFoodManager.GetFoodData(index).UpgradeCost;

            foodLevelText[index].text = "Lv. " + foodLevel.ToString();
            foodProfitText[index].text = foodProfit.ToString();
            upgradeCostText[index].text = upgradeCost.ToString();

            UpdateCurrencyAndButtonCheck();
        }

        private void LearnRecipeTextUpdate(object obj) // update the foods stats text after each upgrade
        {
            int index = (int)obj;
            int learnCost = DBFoodManager.GetFoodData(index).UnlockCost;
            learnRecipeCostText[index].text = learnCost.ToString();
            BuyButtonsCheck();
        }

        private void OnHireUpdate(object obj) // update stats after hiring
        {
            int index = (int)obj;
            int hireCost = DBFoodManager.GetFoodData(index).HireCost;
            int cookFoodTimes = DBFoodManager.GetFoodData(index).CookFoodTimes;
            int bakersCount = DBFoodManager.GetFoodData(index).BakersCount;

            bakersCountText[index].text = bakersCount.ToString() + "x";
            cookFoodTimesText[index].text = cookFoodTimes.ToString() + "x";
            hireCostText[index].text = hireCost.ToString();

            UpdateCurrencyAndButtonCheck();
        }

        #region Cooking

        #region Active Cooking
        private void ActiveCookingUIStart(object obj)
        {
            int index = (int)obj;
            var foodData = DBFoodManager.GetFoodData(index);
            float cookingTime = foodData.CookingTime;
            foodData.IsOnCooldown = true;
            CookFoodButtonCheck();
            CookingUISetUp(timeLeftToCook, index, cookingTime, cookingSliderBar, cookingTimeText, DBTweenTypes.ActiveCookingAnim);
            var countdownTween = CookingTweenTimer(index, timeLeftToCook, DBTweenTypes.ActiveCookingTimer);
            CookingUITweenOnUpdate(index, timeLeftToCook, countdownTween, foodData, remainingCookingTime, cookingTimeText, CookingType.ActiveCooking);
            countdownTween.OnComplete(() => OnTimerComplete(index));
        }

        private void ActiveCookingUIAfterPause(int index)
        {
            var foodData = DBFoodManager.GetFoodData(index);
            float cookingTime = foodData.CookingTime;
            offlineTime = DBManager.Instance.TimerManager.GetLastOfflineTimeSeconds();
            timeLeftToCook[index] = foodData.RemainingCookingTime - offlineTime;

            if (timeLeftToCook[index] > 0)
            {
                foodData.IsOnCooldown = true;
                CookFoodButtonCheck();
                CookingUISetUpAfterPause(foodData, index, cookingTime, timeLeftToCook, DBTweenTypes.ActiveCookingAnim, cookingSliderBar, cookingTimeText, CookingType.ActiveCooking);
                var countdownTween = CookingTweenTimer(index, timeLeftToCook, DBTweenTypes.ActiveCookingTimer);
                CookingUITweenOnUpdate(index, timeLeftToCook, countdownTween, foodData, remainingCookingTime, cookingTimeText, CookingType.ActiveCooking);
                countdownTween.OnComplete(() => OnTimerComplete(index));
            }

            else
            {
                ResetActiveCookingUI(foodData, index, cookingSliderBar, cookingTimeText);
            }

            BuyButtonsCheck();
        }

        private void OnTimerComplete(int index)
        {
            var foodData = DBFoodManager.GetFoodData(index);
            ResetActiveCookingUI(foodData, index, cookingSliderBar, cookingTimeText);
            ActiveCookingCompleteReward(foodData, index);
        }
        #endregion

        #region Idle Baker Cooking
        private void BakerStartTimer(object obj)
        {
            int index = (int)obj;
            var foodData = DBFoodManager.GetFoodData(index);
            float bakerCookingTime = foodData.BakerCookingTime;
            foodData.IsAutoOnCooldown = true;
            CookingUISetUp(bakerTimeLeftCooking, index, bakerCookingTime, bakerSliderBar, bakerTimeText, DBTweenTypes.BakerCookingAnim);
            var countdownTween = CookingTweenTimer(index, bakerTimeLeftCooking, DBTweenTypes.BakerCookingTimer);
            CookingUITweenOnUpdate(index, bakerTimeLeftCooking, countdownTween, foodData, remainingBakerTime, bakerTimeText, CookingType.BakerCooking);
            countdownTween.OnComplete(() => OnBakerTimerComplete(index));
        }

        private void BakerCookingTimerAfterPause(int index)
        {
            var foodData = DBFoodManager.GetFoodData(index);
            float bakerCookingTime = foodData.BakerCookingTime;
            bakerOfflineTime = DBManager.Instance.TimerManager.GetLastOfflineTimeSeconds();
            bakerTimeLeftCooking[index] = GetBakerTimeLeftCooking(
                foodData,
                bakerCookingTime,
                bakerOfflineTime
            );
            foodData.IsAutoOnCooldown = true;
            CookingUISetUpAfterPause(
                foodData,
                index,
                bakerCookingTime,
                bakerTimeLeftCooking,
                DBTweenTypes.BakerCookingAnim,
                bakerSliderBar,
                bakerTimeText,
                CookingType.BakerCooking
            );
            var countdownTween = CookingTweenTimer(
                index,
                bakerTimeLeftCooking,
                DBTweenTypes.BakerCookingTimer
            );
            int previousTime = (int)bakerTimeLeftCooking[index];
            CookingUITweenOnUpdate(
                index,
                bakerTimeLeftCooking,
                countdownTween,
                foodData,
                remainingBakerTime,
                bakerTimeText,
                CookingType.BakerCooking
            );
            countdownTween.OnComplete(() => OnBakerTimerComplete(index));
        }

        private void OnBakerTimerComplete(int index)
        {
            var foodData = DBFoodManager.GetFoodData(index);
            BakerCookingCompleteReward(foodData, index);
        }
        #endregion

        private void CookingPauseCheck(object isPaused)
        {
            if (!(bool)isPaused)
            {
                ResumeCooking();
            }

            else
            {
                KillAllCookingTweens();
            }
        }
        #endregion

        private void MoneyTextToastAfterCooking(object obj) // toasting profit text after cooking (ACTIVE cooking)
        {
            CookFoodButtonCheck();
            int index = (int)obj;
            int foodProfit = DBFoodManager.GetFoodData(index).Profit * DoubleProfitComponent.doubleProfitMultiplier;
            MoneyToasting(foodProfit, PoolNames.MoneyToast);
            BuyButtonsCheck();
        }

        private void MoneyTextToastAfterAutoCooking(object obj) // toasting profit text after cooking (PASSIVE cooking)
        {
            int index = (int)obj;
            int foodProfit = DBFoodManager.GetFoodData(index).Profit * DBFoodManager.GetFoodData(index).CookFoodTimes * DoubleProfitComponent.doubleProfitMultiplier;
            MoneyToasting(foodProfit, PoolNames.MoneyToast);
            BuyButtonsCheck();
        }

        private void SpendUpgradeMoneyTextToast(object obj)
        {
            int index = (int)obj;
            int upgradeCost = DBFoodManager.GetFoodData(index).UpgradeCost;
            MoneyToasting(upgradeCost, PoolNames.SpendMoneyToast);
        }

        private void SpendLearnRecipeMoneyTextToast(object obj)
        {
            int index = (int)obj;
            int learnCost = DBFoodManager.GetFoodData(index).UnlockCost;
            cookButtonAnimation[index].alpha = 1;
            MoneyToasting(learnCost, PoolNames.SpendMoneyToast);
            UpdateCurrencyAndButtonCheck();
        }

        private void SpendHireMoneyTextToast(object obj)
        {
            int index = (int)obj;
            int hireCost = DBFoodManager.GetFoodData(index).HireCost;
            MoneyToasting(hireCost, PoolNames.SpendMoneyToast);
        }

        private void BuyButtonsCheck()
        {
            UpgradeButtonsCheck();
            HireButtonCheck();
            LearnRecipeButtonCheck();
        }

        private void UpgradeButtonsCheck()
        {
            for (int i = 0; i < upgradeButtons.Length; i++)
            {
                int upgradeCost = DBFoodManager.GetFoodData(i).UpgradeCost;
                if (currencyManager.currencySaveData.CurrencyAmount >= upgradeCost)
                {
                    upgradeButtons[i].interactable = true;
                }
                else
                {
                    upgradeButtons[i].interactable = false;
                }
            }
        }

        private void LearnRecipeButtonCheck()
        {
            for (int i = 0; i < learnRecipeButtons.Length; i++)
            {
                int learnCost = DBFoodManager.GetFoodData(i).UnlockCost;
                if (currencyManager.currencySaveData.CurrencyAmount >= learnCost)
                {
                    learnRecipeButtons[i].interactable = true;
                    learnRecipeText[i].color = Color.white;
                    coinIcons[i].color = Color.white;
                }
                else
                {
                    learnRecipeButtons[i].interactable = false;
                    learnRecipeText[i].color = Color.gray;
                    coinIcons[i].color = Color.gray;
                }
            }
        }

        private void HireButtonCheck()
        {
            for (int i = 0; i < hireButtons.Length; i++)
            {
                int hireCost = DBFoodManager.GetFoodData(i).HireCost;
                bool isLocked = DBFoodManager.GetFoodData(i).IsFoodLocked;

                if (currencyManager.currencySaveData.CurrencyAmount >= hireCost && isLocked == false)
                {
                    hireButtons[i].interactable = true;
                }
                else
                {
                    hireButtons[i].interactable = false;
                }
            }
        }

        private void CookFoodButtonCheck()
        {
            for (int i = 0; i < cookFoodButtons.Length; i++)
            {
                bool isOnCooldown = DBFoodManager.GetFoodData(i).IsOnCooldown;
                if (isOnCooldown == false)
                {
                    cookFoodButtons[i].interactable = true;
                    cookButtonAnimation[i].alpha = maxValue;
                }
                else
                {
                    cookFoodButtons[i].interactable = false;
                    cookButtonAnimation[i].alpha = minValue;
                }
            }
        }

        private void InitialCookingUI(int i, CookingType cookingType)
        {
            switch (cookingType)
            {
                case CookingType.ActiveCooking:
                    cookingSliderBar[i].value = minValue;
                    //cookingTimeText[i].text = TimeSpan.FromSeconds(DBFoodManager.GetFoodData(i).CookingTime).ToString("mm':'ss");
                    cookingTimeText[i].text = DBExtension.GetFormattedTimeSpan((int)(DBFoodManager.GetFoodData(i).CookingTime));
                    break;
                case CookingType.BakerCooking:
                    bakerSliderBar[i].value = minValue;
                    //bakerTimeText[i].text = TimeSpan.FromSeconds(DBFoodManager.GetFoodData(i).BakerCookingTime).ToString("mm':'ss");
                    bakerTimeText[i].text = DBExtension.GetFormattedTimeSpan((int)(DBFoodManager.GetFoodData(i).BakerCookingTime));
                    break;
                default:
                    throw new ArgumentException("Invalid CookingType value");
            }
        }

        private void KillAllCookingTweens()
        {
            for (int i = 0; i < DBFoodManager.FOOD_COUNT; i++)
            {
                DOTween.Kill(DBTweenTypes.ActiveCookingAnim + i);
                DOTween.Kill(DBTweenTypes.ActiveCookingTimer + i);
                DOTween.Kill(DBTweenTypes.BakerCookingTimer + i);
                DOTween.Kill(DBTweenTypes.BakerCookingAnim + i);
            }
        }

        private Tween CookingTweenTimer(int index, float[] timeLeft, DBTweenTypes tweenType)
        {
            return
            DOTween.To(() => timeLeft[index], x => timeLeft[index] = x, minValue, timeLeft[index]).SetEase(Ease.Linear).SetId(tweenType + index);
        }

        private void ActiveCookingCompleteReward(FoodData foodData, int index)
        {
            int profit = foodData.Profit * DoubleProfitComponent.doubleProfitMultiplier;
            currencyManager.UpdateCurrency(profit);
            InvokeEvent(DBEventNames.MoneyToastOnCook, index);
        }

        private void ResetActiveCookingUI(FoodData foodData, int index, Slider[] cookingSlider, TMP_Text[] timeText)
        {
            //timeText[index].text = TimeSpan.FromSeconds(foodData.CookingTime).ToString("mm':'ss");
            timeText[index].text = DBExtension.GetFormattedTimeSpan((int)foodData.CookingTime);
            foodData.IsOnCooldown = false;
            cookingSlider[index].DOValue(cookingSlider[index].minValue, minValue).SetEase(Ease.Linear);
            foodData.RemainingCookingTime = foodData.CookingTime;
            CookFoodButtonCheck();
            DBManager.Instance.SaveManager.Save(DBFoodManager.foods);
        }

        private void BakerCookingCompleteReward(FoodData foodData, int index)
        {
            foodData.RemainingBakerCookingTime = foodData.BakerCookingTime;
            bakerSliderBar[index].DOValue(bakerSliderBar[index].minValue, minValue).SetEase(Ease.Linear);
            int profit = foodData.Profit * DoubleProfitComponent.doubleProfitMultiplier * foodData.CookFoodTimes + (foodData.CookFoodTimes == 0 ? foodData.Profit : 0);
            currencyManager.UpdateCurrency(profit);
            foodData.IsAutoOnCooldown = false;
            //bakerTimeText[index].text = TimeSpan.FromSeconds(foodData.BakerCookingTime).ToString("mm':'ss");
            bakerTimeText[index].text = DBExtension.GetFormattedTimeSpan((int)foodData.BakerCookingTime);
            InvokeEvent(DBEventNames.MoneyToastOnAutoCook, index);
            cookingManager.AutoCookFood(index);
            BuyButtonsCheck();
            DBManager.Instance.SaveManager.Save(DBFoodManager.foods);
        }

        private void CookingUISetUp(float[] timeLeft, int index, float cookingTime, Slider[] cookingSlider, TMP_Text[] timeText, DBTweenTypes tweenType)
        {
            timeLeft[index] = cookingTime;
            //timeText[index].text = timeLeft[index].ToString("mm':'ss");
            timeText[index].text = DBExtension.GetFormattedTimeSpan((int)timeLeft[index]);
            cookingSlider[index].value = cookingSlider[index].minValue;
            cookingSlider[index].DOValue(maxValue, timeLeft[index]).SetEase(Ease.Linear).SetId(tweenType + index);
        }

        private void CookingUISetUpAfterPause(
        FoodData foodData,
        int index,
        float cookingTime,
        float[] timeLeft,
        DBTweenTypes tweenType,
        Slider[] cookingSlider,
        TMP_Text[] timeText,
        CookingType cookingType)
        {
            switch (cookingType)
            {
                case CookingType.ActiveCooking:
                    foodData.RemainingCookingTime = timeLeft[index];
                    break;
                case CookingType.BakerCooking:
                    foodData.RemainingBakerCookingTime = timeLeft[index];
                    break;
                default:
                    throw new ArgumentException("Invalid CookingType value");
            }

            //timeText[index].text = timeLeft[index].ToString("mm':'ss");
            timeText[index].text = DBExtension.GetFormattedTimeSpan((int)timeLeft[index]);
            float fillValue = (cookingTime - timeLeft[index]) / cookingTime;
            cookingSlider[index].value = fillValue;
            cookingSlider[index].DOValue(maxValue, timeLeft[index]).SetEase(Ease.Linear).SetId(tweenType + index);
        }

        private void CookingUITweenOnUpdate(int index, float[] timeLeft, Tween timerTween, FoodData foodData, TimeSpan remainingTime, TMP_Text[] timeText, CookingType cookingType)
        {
            int previousTime = (int)timeLeft[index];
            timerTween.OnUpdate(() =>
            {
                remainingTime = TimeSpan.FromSeconds(timeLeft[index]);
                if (previousTime != (int)timeLeft[index])
                {
                    switch (cookingType)
                    {
                        case CookingType.ActiveCooking:
                            foodData.RemainingCookingTime = timeLeft[index];
                            break;
                        case CookingType.BakerCooking:
                            foodData.RemainingBakerCookingTime = timeLeft[index];
                            break;
                        default:
                            throw new ArgumentException("Invalid CookingType value");
                    }
                    DBManager.Instance.SaveManager.Save(DBFoodManager.foods);
                    previousTime = (int)timeLeft[index];
                }
                timeText[index].text = DBExtension.FormatTimeSpan(remainingTime);
            });
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

        private void ResumeCooking()
        {
            for (int i = 0; i < DBFoodManager.FOOD_COUNT; i++)
            {
                if (DBFoodManager.GetFoodData(i).IsOnCooldown == true)
                {
                    ActiveCookingUIAfterPause(i);
                }
                if (DBFoodManager.GetFoodData(i).IsIdleFood == true)
                {
                    BakerCookingTimerAfterPause(i);
                }
            }
        }

        private void MoneyToastPoolInitialization()
        {
            Manager.PoolManager.InitPool("MoneyToast", moneyTextToastAmount, moneyToastPosition);
            Manager.PoolManager.InitPool("SpendMoneyToast", moneyTextToastAmount, moneyToastPosition);
        }

        private void UpdateCurrencyAndButtonCheck()
        {
            currencyManager.UpdateCurrency(currencyManager.startingCurrency);
            BuyButtonsCheck();
        }

        private void MoneyToasting(int moneyAmount, PoolNames poolName)
        {
            var moneyToast = (DBTweenMoneyComponent)Manager.PoolManager.GetPoolable(poolName);
            Vector3 toastPosition = moneyToastPosition.position + Vector3.up * 3;
            moneyToast.transform.position = toastPosition;

            switch (poolName)
            {
                case PoolNames.MoneyToast:
                    moneyToast.Init(moneyAmount);
                    break;
                case PoolNames.SpendMoneyToast:
                    moneyToast.SpendInit(moneyAmount);
                    break;
                default:
                    throw new ArgumentException("Invalid PoolName value");
            }
        }
    }

    public enum DBTweenTypes
    {
        ActiveCookingAnim,
        ActiveCookingTimer,
        BakerCookingTimer,
        BakerCookingAnim,
        DoubleProfit
    }

    public enum CookingType
    {
        ActiveCooking,
        BakerCooking
    }
}