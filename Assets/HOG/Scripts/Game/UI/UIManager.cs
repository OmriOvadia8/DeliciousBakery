using Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;
using System.Collections.Generic;

namespace Game
{
    public class UIManager : HOGLogicMonoBehaviour
    {
        private readonly Dictionary<int, Tweener> foodLoadingBarTweens = new(); // DOTween dictionary - Tween for each cooking food loading bar
        private readonly Dictionary<int, Tweener> bakerLoadingBarTweens = new(); // DOTween dictionary - Tween for each cooking baker loading bar

        private readonly float minValue = 0f;
        private readonly float maxValue = 1f;

        private float[] savedCookingTime;

        [Header("Components")]
        [SerializeField] HOGTweenMoneyComponent moneyComponent;
        [SerializeField] HOGTweenMoneyComponent SpendMoneyComponent;
        [SerializeField] RectTransform moneyToastPosition;

        [Header("Managers")]
        [SerializeField] FoodManager foodManager;
        [SerializeField] HOGMoneyHolder moneyHolder;

        [Header("Buttons")]
        [SerializeField] Button[] cookFoodButtons;
        [SerializeField] Button[] upgradeButtons;
        [SerializeField] Button[] hireButtons;
        [SerializeField] Button[] learnRecipeButtons;
        [SerializeField] CanvasGroup[] cookButtonAnimation;

        [Header("Texts")]
        [SerializeField] TMP_Text moneyText;
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
            AddListener(HOGEventNames.OnCurrencySet, OnMoneyUpdate);
            AddListener(HOGEventNames.OnUpgraded, OnUpgradeUpdate);
            AddListener(HOGEventNames.OnCookFood, CookingLoadingBarAnimation);
            AddListener(HOGEventNames.OnCookFood, CookingTimer);
            AddListener(HOGEventNames.MoneyToastOnCook, MoneyTextToastAfterCooking);
            AddListener(HOGEventNames.MoneyToastOnAutoCook, MoneyTextToastAfterAutoCooking);
            AddListener(HOGEventNames.OnUpgradeMoneySpentToast, SpendUpgradeMoneyTextToast);
            AddListener(HOGEventNames.OnHireMoneySpentToast, SpendHireMoneyTextToast);
            AddListener(HOGEventNames.OnHired, OnHireUpdate);
            AddListener(HOGEventNames.OnAutoCookFood, BakerCookingLoadingBarAnimation);
            AddListener(HOGEventNames.OnAutoCookFood, BakerCookingTimer);
            AddListener(HOGEventNames.OnLearnRecipe, LearnRecipeTextUpdate);
            AddListener(HOGEventNames.OnLearnRecipeSpentToast, SpendLearnRecipeMoneyTextToast);
            AddListener(HOGEventNames.OnAutoCookOnResume, BakerLoadingBarAnimationOnResume);
            AddListener(HOGEventNames.OnAutoCookOnResume, BakerCookingTimerOnResume);

            
        }

        private void Start()
        {
            Manager.PoolManager.InitPool("MoneyToast", 30, moneyToastPosition);
            Manager.PoolManager.InitPool("SpendMoneyToast", 20, moneyToastPosition);
            savedCookingTime = new float[FoodManager.FOOD_COUNT];
            OnGameLoad();
        }

        private void OnDisable()
        {
            RemoveListener(HOGEventNames.OnCurrencySet, OnMoneyUpdate);
            RemoveListener(HOGEventNames.OnUpgraded, OnUpgradeUpdate);
            RemoveListener(HOGEventNames.OnCookFood, CookingLoadingBarAnimation);
            RemoveListener(HOGEventNames.OnCookFood, CookingTimer);
            RemoveListener(HOGEventNames.MoneyToastOnCook, MoneyTextToastAfterCooking);
            RemoveListener(HOGEventNames.MoneyToastOnAutoCook, MoneyTextToastAfterAutoCooking);
            RemoveListener(HOGEventNames.OnUpgradeMoneySpentToast, SpendUpgradeMoneyTextToast);
            RemoveListener(HOGEventNames.OnHireMoneySpentToast, SpendHireMoneyTextToast);
            RemoveListener(HOGEventNames.OnHired, OnHireUpdate);
            RemoveListener(HOGEventNames.OnAutoCookFood, BakerCookingLoadingBarAnimation);
            RemoveListener(HOGEventNames.OnAutoCookFood, BakerCookingTimer);
            RemoveListener(HOGEventNames.OnLearnRecipe, LearnRecipeTextUpdate);
            RemoveListener(HOGEventNames.OnLearnRecipeSpentToast, SpendLearnRecipeMoneyTextToast);
            RemoveListener(HOGEventNames.OnAutoCookOnResume, BakerLoadingBarAnimationOnResume);
            RemoveListener(HOGEventNames.OnAutoCookOnResume, BakerCookingTimerOnResume);
        }

        private void OnGameLoad()
        {
            moneyText.text = $"{moneyHolder.currencySaveData.CurrencyAmount:N0}";

            for (int i = 0; i < FoodManager.FOOD_COUNT; i++)
            {
                cookingSliderBar[i].value = minValue;
                float cookingTime = GetFoodData(i).CookingTime;
                cookingTimeText[i].text = TimeSpan.FromSeconds(cookingTime).ToString("mm':'ss"); // set the cooking time in the timer text

                bakerSliderBar[i].value = minValue;
                float bakerCookingTime = GetFoodData(i).CookingTime * CookingManager.BAKER_TIME_MULTIPLIER;

                if (Manager.TimerManager.GetLastOfflineTimeSeconds() > 0)
                {
                    float adjustedBakerCookingTime = bakerCookingTime;
                    if (savedCookingTime[i] > 0)
                    {
                        adjustedBakerCookingTime -= savedCookingTime[i];
                    }
                    savedCookingTime[i] = adjustedBakerCookingTime - (Manager.TimerManager.GetLastOfflineTimeSeconds() % adjustedBakerCookingTime + adjustedBakerCookingTime) % adjustedBakerCookingTime;
                    bakerSliderBar[i].value = savedCookingTime[i] / adjustedBakerCookingTime;

                    TimeSpan adjustedBakingTime = TimeSpan.FromSeconds(adjustedBakerCookingTime - savedCookingTime[i]);
                    bakerTimeText[i].text = adjustedBakingTime.ToString("mm':'ss"); // set the adjusted baking time in the timer text
                }

                if (GetFoodData(i).IsIdleFood == false)
                {
                    cookingSliderBar[i].value = minValue;
                    cookingTime = GetFoodData(i).CookingTime;
                    cookingTimeText[i].text = TimeSpan.FromSeconds(cookingTime).ToString("mm':'ss"); // set the cooking time in the timer text

                    bakerSliderBar[i].value = minValue;
                    bakerCookingTime = GetFoodData(i).CookingTime * CookingManager.BAKER_TIME_MULTIPLIER;
                    bakerTimeText[i].text = TimeSpan.FromSeconds(bakerCookingTime).ToString("mm':'ss"); // set the baking time in the timer text
                }

                if (GetFoodData(i).IsFoodLocked == true)
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

        private void OnUpgradeUpdate(object obj) // update the foods stats text after each upgrade
        {
            int foodLevel = GameLogic.UpgradeManager.GetUpgradeableByID(UpgradeablesTypeID.Food, (int)obj).CurrentLevel;
            int foodProfit = GetFoodData((int)obj).Profit;
            int upgradeCost = GetFoodData((int)obj).UpgradeCost;

            foodLevelText[(int)obj].text = "Lv. " + foodLevel.ToString();
            foodProfitText[(int)obj].text = foodProfit.ToString();
            upgradeCostText[(int)obj].text = upgradeCost.ToString();

            moneyHolder.UpdateCurrency(moneyHolder.startingCurrency);
            BuyButtonsCheck();
        }

        private void LearnRecipeTextUpdate(object obj) // update the foods stats text after each upgrade
        {
            int learnCost = GetFoodData((int)obj).UnlockCost;

            learnRecipeCostText[(int)obj].text = learnCost.ToString();

            BuyButtonsCheck();
        }

        private void OnHireUpdate(object obj) // update stats after hiring
        {
            int hireCost = GetFoodData((int)obj).HireCost;
            int cookFoodTimes = GetFoodData((int)obj).CookFoodTimes;
            int bakersCount = GetFoodData((int)obj).BakersCount;

            bakersCountText[(int)obj].text = bakersCount.ToString() + "x";
            cookFoodTimesText[(int)obj].text = cookFoodTimes.ToString() + "x";
            hireCostText[(int)obj].text = hireCost.ToString();

            moneyHolder.UpdateCurrency(moneyHolder.startingCurrency);
            BuyButtonsCheck();
        }

        private void CookingLoadingBarAnimation(object obj) // activates loading bar with DOTween (ACTIVE cooking)
        {
            CookFoodButtonCheck();
            float foodCookingTime = GetFoodData((int)obj).CookingTime;

            cookingSliderBar[(int)obj].value = minValue;
            foodLoadingBarTweens[(int)obj] = cookingSliderBar[(int)obj].DOValue(maxValue, foodCookingTime);

            foodLoadingBarTweens[(int)obj].OnComplete(() =>
            {
                cookingSliderBar[(int)obj].value = minValue;
                cookingTimeText[(int)obj].text = FormatTimeSpan(TimeSpan.FromSeconds(foodCookingTime));
            });
        }

        private void CookingTimer(object obj) // activates the cooking timer countdown (ACTIVE cooking)
        {
            float foodCookingTime = GetFoodData((int)obj).CookingTime;
            TimeSpan timeLeft = TimeSpan.FromSeconds(foodCookingTime);

            string timeLeftString = FormatTimeSpan(timeLeft);
            cookingTimeText[(int)obj].text = timeLeftString;

            foodLoadingBarTweens[(int)obj].OnUpdate(() =>
            {
                timeLeft = TimeSpan.FromSeconds(foodLoadingBarTweens[(int)obj].Duration() - foodLoadingBarTweens[(int)obj].Elapsed());
                timeLeftString = FormatTimeSpan(timeLeft);
                cookingTimeText[(int)obj].text = timeLeftString;
            });
        }

        private void BakerCookingLoadingBarAnimation(object obj) // activates loading bar with DOTween (PASSIVE cooking)
        {
            float foodCookingTime = GetFoodData((int)obj).CookingTime * CookingManager.BAKER_TIME_MULTIPLIER;

            bakerSliderBar[(int)obj].value = minValue;
            bakerLoadingBarTweens[(int)obj] = bakerSliderBar[(int)obj].DOValue(maxValue, foodCookingTime);

            bakerLoadingBarTweens[(int)obj].OnComplete(() =>
            {
                bakerSliderBar[(int)obj].value = minValue;
                bakerTimeText[(int)obj].text = FormatTimeSpan(TimeSpan.FromSeconds(foodCookingTime));
            });
        }

        private void BakerCookingTimer(object obj) // activates the cooking timer countdown (PASSIVE cooking)
        {
            float foodCookingTime = GetFoodData((int)obj).CookingTime * CookingManager.BAKER_TIME_MULTIPLIER;
            TimeSpan timeLeft = TimeSpan.FromSeconds(foodCookingTime);

            string timeLeftString = FormatTimeSpan(timeLeft);
            bakerTimeText[(int)obj].text = timeLeftString;

            bakerLoadingBarTweens[(int)obj].OnUpdate(() =>
            {
                timeLeft = TimeSpan.FromSeconds(bakerLoadingBarTweens[(int)obj].Duration() - bakerLoadingBarTweens[(int)obj].Elapsed());
                timeLeftString = FormatTimeSpan(timeLeft);
                bakerTimeText[(int)obj].text = timeLeftString;
            });
        }

        private void BakerLoadingBarAnimationOnResume(object obj)
        {
            var foodData = GetFoodData((int)obj);
            int offlineTime = Manager.TimerManager.GetLastOfflineTimeSeconds();
            float cookingTime = foodData.CookingTime * CookingManager.BAKER_TIME_MULTIPLIER;

            float timeLeftToCook = cookingTime - (offlineTime % cookingTime + cookingTime) % cookingTime;

            float fillAmount = maxValue - (timeLeftToCook / cookingTime);
            float timeToFill = timeLeftToCook;

            bakerSliderBar[(int)obj].value = fillAmount;

            bakerLoadingBarTweens[(int)obj] = bakerSliderBar[(int)obj].DOValue(maxValue, timeToFill * bakerSliderBar[(int)obj].value);

            bakerLoadingBarTweens[(int)obj].OnComplete(() =>
            {
                bakerSliderBar[(int)obj].value = minValue;
                bakerTimeText[(int)obj].text = FormatTimeSpan(TimeSpan.FromSeconds(cookingTime));
            });
        }

        private void BakerCookingTimerOnResume(object obj)
        {
            var foodData = GetFoodData((int)obj);
            int offlineTime = Manager.TimerManager.GetLastOfflineTimeSeconds();
            float cookingTime = foodData.CookingTime * CookingManager.BAKER_TIME_MULTIPLIER;
            float timeLeftToCook = cookingTime - (offlineTime % cookingTime + cookingTime) % cookingTime;

            bakerLoadingBarTweens[(int)obj] = bakerSliderBar[(int)obj].DOValue(maxValue, timeLeftToCook);

            TimeSpan timeLeft = TimeSpan.FromSeconds(timeLeftToCook);
            string timeLeftString = FormatTimeSpan(timeLeft);
            bakerTimeText[(int)obj].text = timeLeftString;

            bakerLoadingBarTweens[(int)obj].OnUpdate(() =>
            {
                timeLeft = TimeSpan.FromSeconds(bakerLoadingBarTweens[(int)obj].Duration() - bakerLoadingBarTweens[(int)obj].Elapsed());
                timeLeftString = FormatTimeSpan(timeLeft);
                bakerTimeText[(int)obj].text = timeLeftString;
            });
        }

        private void MoneyTextToastAfterCooking(object obj) // toasting profit text after cooking (ACTIVE cooking)
        {
            CookFoodButtonCheck();
            int foodIndex = (int)obj;
            int foodProfit = GetFoodData(foodIndex).Profit;
            var moneyToast = (HOGTweenMoneyComponent)Manager.PoolManager.GetPoolable(PoolNames.MoneyToast);

            Vector3 toastPosition = moneyToastPosition.position + Vector3.up * 3;
            moneyToast.transform.position = toastPosition;

            moneyToast.Init(foodProfit);

            BuyButtonsCheck();
        }

        private void MoneyTextToastAfterAutoCooking(object obj) // toasting profit text after cooking (PASSIVE cooking)
        {
            int foodIndex = (int)obj;
            int foodProfit = GetFoodData(foodIndex).Profit * GetFoodData(foodIndex).CookFoodTimes;
            var moneyToast = (HOGTweenMoneyComponent)Manager.PoolManager.GetPoolable(PoolNames.MoneyToast);

            Vector3 toastPosition = moneyToastPosition.position + Vector3.up * 3;
            moneyToast.transform.position = toastPosition;

            moneyToast.Init(foodProfit);

            BuyButtonsCheck();
        }

        private void SpendUpgradeMoneyTextToast(object obj)
        {
            int foodIndex = (int)obj;
            int upgradeCost = GetFoodData(foodIndex).UpgradeCost;
            var moneyToast = (HOGTweenMoneyComponent)Manager.PoolManager.GetPoolable(PoolNames.SpendMoneyToast);

            Vector3 toastPosition = moneyToastPosition.position + Vector3.up * 3;
            moneyToast.transform.position = toastPosition;

            moneyToast.SpendInit(upgradeCost);
        }

        private void SpendLearnRecipeMoneyTextToast(object obj)
        {
            int foodIndex = (int)obj;
            int learnCost = GetFoodData(foodIndex).UnlockCost;
            cookButtonAnimation[foodIndex].alpha = 1;

            var moneyToast = (HOGTweenMoneyComponent)Manager.PoolManager.GetPoolable(PoolNames.SpendMoneyToast);

            Vector3 toastPosition = moneyToastPosition.position + Vector3.up * 3;
            moneyToast.transform.position = toastPosition;

            moneyToast.SpendInit(learnCost);

            moneyHolder.UpdateCurrency(moneyHolder.startingCurrency);

            BuyButtonsCheck();
        }

        private void SpendHireMoneyTextToast(object obj)
        {
            int foodIndex = (int)obj;
            int hireCost = GetFoodData(foodIndex).HireCost;
            var moneyToast = (HOGTweenMoneyComponent)Manager.PoolManager.GetPoolable(PoolNames.SpendMoneyToast);

            Vector3 toastPosition = moneyToastPosition.position + Vector3.up * 3;
            moneyToast.transform.position = toastPosition;

            moneyToast.SpendInit(hireCost);
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
                int upgradeCost = GetFoodData(i).UpgradeCost;
                if (moneyHolder.currencySaveData.CurrencyAmount >= upgradeCost)
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
                int learnCost = GetFoodData(i).UnlockCost;
                if (moneyHolder.currencySaveData.CurrencyAmount >= learnCost)
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
                int hireCost = GetFoodData(i).HireCost;
                bool isLocked = GetFoodData(i).IsFoodLocked;

                if (moneyHolder.currencySaveData.CurrencyAmount >= hireCost && isLocked == false)
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
                bool isOnCooldown = GetFoodData(i).IsOnCooldown;
                if (isOnCooldown == false)
                {
                    cookFoodButtons[i].interactable = true;
                    cookButtonAnimation[i].alpha = 1;
                }
                else
                {
                    cookFoodButtons[i].interactable = false;
                    cookButtonAnimation[i].alpha = 0;
                }
            }
        }

        private FoodData GetFoodData(int index)
        {
            return foodManager.GetFoodData(index);
        }

        private string FormatTimeSpan(TimeSpan timeSpan)
        {
            return string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        }
    }
}