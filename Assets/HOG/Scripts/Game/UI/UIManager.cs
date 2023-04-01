using Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;

namespace Game
{
    public class UIManager : HOGLogicMonoBehaviour
    {
        private readonly float minValue = 0f;
        private readonly float maxValue = 1f;

        private int offlineTime;
        private int bakerOfflineTime;
        private float[] timeLeftToCook = new float[FoodManager.FOOD_COUNT];
        private float[] bakerTimeLeftCooking = new float[FoodManager.FOOD_COUNT];

        TimeSpan remainingCookingTime;
        TimeSpan remainingBakerTime;

        [Header("Components")]
        [SerializeField] HOGTweenMoneyComponent moneyComponent;
        [SerializeField] HOGTweenMoneyComponent SpendMoneyComponent;
        [SerializeField] RectTransform moneyToastPosition;

        [Header("Managers")]
        [SerializeField] FoodManager foodManager;
        [SerializeField] HOGMoneyHolder moneyHolder;
        [SerializeField] CookingManager cookingManager;

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
            AddListener(HOGEventNames.OnCookFood, StartTimer);
            AddListener(HOGEventNames.OnPause, CookingPauseCheck);
            AddListener(HOGEventNames.MoneyToastOnCook, MoneyTextToastAfterCooking);
            AddListener(HOGEventNames.MoneyToastOnAutoCook, MoneyTextToastAfterAutoCooking);
            AddListener(HOGEventNames.OnUpgradeMoneySpentToast, SpendUpgradeMoneyTextToast);
            AddListener(HOGEventNames.OnHireMoneySpentToast, SpendHireMoneyTextToast);
            AddListener(HOGEventNames.OnHired, OnHireUpdate);
            AddListener(HOGEventNames.OnAutoCookFood, BakerStartTimer);
            AddListener(HOGEventNames.OnLearnRecipe, LearnRecipeTextUpdate);
            AddListener(HOGEventNames.OnLearnRecipeSpentToast, SpendLearnRecipeMoneyTextToast);

        }

        private void Start()
        {
            Manager.PoolManager.InitPool("MoneyToast", 30, moneyToastPosition);
            Manager.PoolManager.InitPool("SpendMoneyToast", 20, moneyToastPosition);
            OnGameLoad();

            CookingPauseCheck(false);
        }

        private void OnDisable()
        {
            RemoveListener(HOGEventNames.OnCurrencySet, OnMoneyUpdate);
            RemoveListener(HOGEventNames.OnUpgraded, OnUpgradeUpdate);
            RemoveListener(HOGEventNames.OnCookFood, StartTimer);
            RemoveListener(HOGEventNames.OnPause, CookingPauseCheck);
            RemoveListener(HOGEventNames.MoneyToastOnCook, MoneyTextToastAfterCooking);
            RemoveListener(HOGEventNames.MoneyToastOnAutoCook, MoneyTextToastAfterAutoCooking);
            RemoveListener(HOGEventNames.OnUpgradeMoneySpentToast, SpendUpgradeMoneyTextToast);
            RemoveListener(HOGEventNames.OnHireMoneySpentToast, SpendHireMoneyTextToast);
            RemoveListener(HOGEventNames.OnHired, OnHireUpdate);
            RemoveListener(HOGEventNames.OnAutoCookFood, BakerStartTimer);
            RemoveListener(HOGEventNames.OnLearnRecipe, LearnRecipeTextUpdate);
            RemoveListener(HOGEventNames.OnLearnRecipeSpentToast, SpendLearnRecipeMoneyTextToast);
        }

        private void OnGameLoad()
        {
            moneyText.text = $"{moneyHolder.currencySaveData.CurrencyAmount:N0}";

            for (int i = 0; i < FoodManager.FOOD_COUNT; i++)
            {
                cookingSliderBar[i].value = minValue;
                float cookingTime = GetFoodData(i).CookingTime;
                cookingTimeText[i].text = TimeSpan.FromSeconds(cookingTime).ToString("mm':'ss"); // set the cooking time in the timer text

                if (GetFoodData(i).IsIdleFood == false)
                {
                    bakerSliderBar[i].value = minValue;
                    float bakerCookingTime = GetFoodData(i).BakerCookingTime;
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

        // .................................................................. ACTIVE COOKING ....................................................................

        private void StartTimer(object obj)
        {
            var foodIndex = foodManager.GetFoodData((int)obj);
            float cookingTime = foodIndex.CookingTime;
            foodIndex.IsOnCooldown = true;

            CookFoodButtonCheck();

            timeLeftToCook[(int)obj] = cookingTime;

            var countdownTween = DOTween.To(() => timeLeftToCook[(int)obj], x => timeLeftToCook[(int)obj] = x, 0, timeLeftToCook[(int)obj]).SetEase(Ease.Linear).SetId("activeCookingTimer" + (int)obj);

            cookingTimeText[(int)obj].text = timeLeftToCook[(int)obj].ToString("mm':'ss");

            cookingSliderBar[(int)obj].value = cookingSliderBar[(int)obj].minValue;

            cookingSliderBar[(int)obj].DOValue(maxValue, timeLeftToCook[(int)obj]).SetEase(Ease.Linear).SetId("activeCookingAnim" + (int)obj);

            int previousTime = (int)timeLeftToCook[(int)obj];

            countdownTween.OnUpdate(() =>
            {
                remainingCookingTime = TimeSpan.FromSeconds(timeLeftToCook[(int)obj]);

                if (previousTime != (int)timeLeftToCook[(int)obj])
                {
                    foodIndex.RemainingCookingTime = timeLeftToCook[(int)obj];
                    HOGManager.Instance.SaveManager.Save(foodManager.foods);
                    previousTime = (int)timeLeftToCook[(int)obj];
                }

                cookingTimeText[(int)obj].text = $"{remainingCookingTime.Minutes:D2}:{remainingCookingTime.Seconds:D2}";
            });

            countdownTween.OnComplete(() => OnTimerComplete((int)obj));
        }

        private void StartTimerAfterPause(object obj)
        {
            var foodIndex = foodManager.GetFoodData((int)obj);
            float cookingTime = foodIndex.CookingTime;
            offlineTime = HOGManager.Instance.TimerManager.GetLastOfflineTimeSeconds();

            timeLeftToCook[(int)obj] = foodIndex.RemainingCookingTime - offlineTime;

            foodIndex.IsOnCooldown = true;

            CookFoodButtonCheck();

            if (timeLeftToCook[(int)obj] > 0)
            {
                foodIndex.RemainingCookingTime = timeLeftToCook[(int)obj];
                var countdownTween = DOTween.To(() => timeLeftToCook[(int)obj], x => timeLeftToCook[(int)obj] = x, 0, timeLeftToCook[(int)obj]).SetEase(Ease.Linear).SetId("activeCookingTimer" + (int)obj);
                cookingTimeText[(int)obj].text = timeLeftToCook[(int)obj].ToString("mm':'ss");
                float fillValue = (cookingTime - timeLeftToCook[(int)obj]) / cookingTime;
                cookingSliderBar[(int)obj].value = fillValue;
                cookingSliderBar[(int)obj].DOValue(maxValue, timeLeftToCook[(int)obj]).SetEase(Ease.Linear).SetId("activeCookingAnim" + (int)obj);

                int previousTime = (int)timeLeftToCook[(int)obj];

                countdownTween.OnUpdate(() =>
                {
                remainingCookingTime = TimeSpan.FromSeconds(timeLeftToCook[(int)obj]);

                if (previousTime != (int)timeLeftToCook[(int)obj])
                {
                    foodIndex.RemainingCookingTime = timeLeftToCook[(int)obj];
                    HOGManager.Instance.SaveManager.Save(foodManager.foods);
                    previousTime = (int)timeLeftToCook[(int)obj];
                }

                cookingTimeText[(int)obj].text = $"{remainingCookingTime.Minutes:D2}:{remainingCookingTime.Seconds:D2}";
                
                });

            countdownTween.OnComplete(() => OnTimerComplete((int)obj));
            }

            else
            {
                cookingTimeText[(int)obj].text = TimeSpan.FromSeconds(cookingTime).ToString("mm':'ss");
                foodIndex.IsOnCooldown = false;
                cookingSliderBar[(int)obj].DOValue(cookingSliderBar[(int)obj].minValue, 0f).SetEase(Ease.Linear);
                foodIndex.RemainingCookingTime = foodIndex.CookingTime;
                HOGManager.Instance.SaveManager.Save(foodManager.foods);
            }

            BuyButtonsCheck();
            CookFoodButtonCheck();
            }

        private void OnTimerComplete(int index)
        {
            var foodIndex = foodManager.GetFoodData(index);
            foodIndex.RemainingCookingTime = foodIndex.CookingTime;
            cookingSliderBar[index].DOValue(cookingSliderBar[index].minValue, 0f).SetEase(Ease.Linear);
            int profit = foodIndex.Profit;
            moneyHolder.UpdateCurrency(profit);
            foodIndex.IsOnCooldown = false;
            cookingTimeText[index].text = TimeSpan.FromSeconds(foodIndex.CookingTime).ToString("mm':'ss");
            InvokeEvent(HOGEventNames.MoneyToastOnCook, index); 
        }
        // ...................................................................................................................................................

        // .......................................................................IDLE COOKING............................................................................

        private void BakerStartTimer(object obj)
        {
            var foodIndex = foodManager.GetFoodData((int)obj);
            float bakerCookingTime = foodIndex.BakerCookingTime;
            foodIndex.IsAutoOnCooldown = true;

            bakerTimeLeftCooking[(int)obj] = bakerCookingTime;

            var countdownTween = DOTween.To(() => bakerTimeLeftCooking[(int)obj], x => bakerTimeLeftCooking[(int)obj] = x, 0, bakerTimeLeftCooking[(int)obj]).SetEase(Ease.Linear).SetId("bakerCookingTimer" + (int)obj);

            bakerTimeText[(int)obj].text = bakerTimeLeftCooking[(int)obj].ToString("mm':'ss");

            bakerSliderBar[(int)obj].value = bakerSliderBar[(int)obj].minValue;

            bakerSliderBar[(int)obj].DOValue(maxValue, bakerTimeLeftCooking[(int)obj]).SetEase(Ease.Linear).SetId("bakerCookingAnim" + (int)obj);

            int previousTime = (int)bakerTimeLeftCooking[(int)obj];

            countdownTween.OnUpdate(() =>
            {
                remainingBakerTime = TimeSpan.FromSeconds(bakerTimeLeftCooking[(int)obj]);

                if (previousTime != (int)bakerTimeLeftCooking[(int)obj])
                {
                    foodIndex.RemainingBakerCookingTime = bakerTimeLeftCooking[(int)obj];
                    HOGManager.Instance.SaveManager.Save(foodManager.foods);
                    previousTime = (int)bakerTimeLeftCooking[(int)obj];
                }

                bakerTimeText[(int)obj].text = $"{remainingBakerTime.Minutes:D2}:{remainingBakerTime.Seconds:D2}";
            });

            countdownTween.OnComplete(() => OnBakerTimerComplete((int)obj));
        }

        private void BakerCookingTimerAfterPause(object obj)
        {
            var foodIndex = foodManager.GetFoodData((int)obj);
            float bakerCookingTime = foodIndex.BakerCookingTime;
            bakerOfflineTime = HOGManager.Instance.TimerManager.GetLastOfflineTimeSeconds();

            if(bakerOfflineTime > foodIndex.RemainingBakerCookingTime)
            {
                float greaterOfflineBakerTime = bakerCookingTime - ((bakerOfflineTime - foodIndex.RemainingBakerCookingTime) % bakerCookingTime);

                bakerTimeLeftCooking[(int)obj] = greaterOfflineBakerTime;
            }

            else
            {
                float LessOfflineBakerTime = foodIndex.RemainingBakerCookingTime - bakerOfflineTime;
                bakerTimeLeftCooking[(int)obj] = LessOfflineBakerTime;
            }

            foodIndex.IsAutoOnCooldown = true;

            foodIndex.RemainingBakerCookingTime = bakerTimeLeftCooking[(int)obj];

            var countdownTween = DOTween.To(() => bakerTimeLeftCooking[(int)obj], x => bakerTimeLeftCooking[(int)obj] = x, 0, bakerTimeLeftCooking[(int)obj]).SetEase(Ease.Linear).SetId("bakerCookingTimer" + (int)obj);
            bakerTimeText[(int)obj].text = bakerTimeLeftCooking[(int)obj].ToString("mm':'ss");
            float fillValue = (bakerCookingTime - bakerTimeLeftCooking[(int)obj]) / bakerCookingTime;
            bakerSliderBar[(int)obj].value = fillValue;
            bakerSliderBar[(int)obj].DOValue(maxValue, bakerTimeLeftCooking[(int)obj]).SetEase(Ease.Linear).SetId("bakerCookingAnim" + (int)obj);

            int previousTime = (int)bakerTimeLeftCooking[(int)obj];

            countdownTween.OnUpdate(() =>
            {
                remainingBakerTime = TimeSpan.FromSeconds(bakerTimeLeftCooking[(int)obj]);

                if (previousTime != (int)bakerTimeLeftCooking[(int)obj])
                {
                    foodIndex.RemainingBakerCookingTime = bakerTimeLeftCooking[(int)obj];
                    HOGManager.Instance.SaveManager.Save(foodManager.foods);
                    previousTime = (int)bakerTimeLeftCooking[(int)obj];
                }

                bakerTimeText[(int)obj].text = $"{remainingBakerTime.Minutes:D2}:{remainingBakerTime.Seconds:D2}";

            });

            countdownTween.OnComplete(() => OnBakerTimerComplete((int)obj));

            BuyButtonsCheck();
        }

        private void OnBakerTimerComplete(int index)
        {
            var foodIndex = foodManager.GetFoodData(index);
            foodIndex.RemainingBakerCookingTime = foodIndex.BakerCookingTime;
            bakerSliderBar[index].DOValue(bakerSliderBar[index].minValue, 0f).SetEase(Ease.Linear);

            if (foodIndex.CookFoodTimes == 0)
            {
                int profit = foodIndex.Profit * (foodIndex.CookFoodTimes + 1);
                moneyHolder.UpdateCurrency(profit);
                foodIndex.IsAutoOnCooldown = false;
                bakerTimeText[index].text = TimeSpan.FromSeconds(foodIndex.BakerCookingTime).ToString("mm':'ss");
                InvokeEvent(HOGEventNames.MoneyToastOnAutoCook, index);
                BuyButtonsCheck();
                cookingManager.AutoCookFood(index);
                HOGManager.Instance.SaveManager.Save(foodManager.foods);
            }

            else
            {
                int profit = foodIndex.Profit * foodIndex.CookFoodTimes;
                foodIndex.IsAutoOnCooldown = false;
                moneyHolder.UpdateCurrency(profit);
                bakerTimeText[index].text = TimeSpan.FromSeconds(foodIndex.BakerCookingTime).ToString("mm':'ss");
                InvokeEvent(HOGEventNames.MoneyToastOnAutoCook, index);
                BuyButtonsCheck();
                cookingManager.AutoCookFood(index);
                HOGManager.Instance.SaveManager.Save(foodManager.foods);
            }  
        }

        // ...................................................................................................................................................
        private void CookingPauseCheck(object isPaused)
        {
            if (!(bool)isPaused)
            {
                for (int i = 0; i < FoodManager.FOOD_COUNT; i++)
                {
                    if (foodManager.GetFoodData(i).IsOnCooldown == true)
                    {
                        StartTimerAfterPause(i);
                    }
                    if(foodManager.GetFoodData(i).IsIdleFood == true)
                    {
                        BakerCookingTimerAfterPause(i);
                    }
                }
            }

            else
            {
                HOGManager.Instance.SaveManager.Save(foodManager.foods);

                for (int i = 0; i < FoodManager.FOOD_COUNT; i++)
                {
                    DOTween.Kill("activeCookingAnim" + i);
                    DOTween.Kill("activeCookingTimer" + i);
                    DOTween.Kill("bakerCookingTimer" + i);
                    DOTween.Kill("bakerCookingAnim" + i);
                }
            }
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