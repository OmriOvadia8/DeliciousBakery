using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using DB_Core;

namespace DB_Game
{
    public class DBAchievementsProgressUIController : FoodDataAccess
    {
        [SerializeField] AchievementsMakeFoodProgressComponents foodProgressComponents;
        [SerializeField] AchievementsHireBakerProgressComponents hireProgressComponents;
        [SerializeField] DBAchievementsController achievementsController;
        [SerializeField] DBAchievementsDataManager claimData;
        private readonly int totalRewardsCount = DBAchievementsController.FoodItemsAchievementsRewards.Length;

        private void Start() => AddClaimButtonListeners();

        private void AddClaimButtonListeners()
        {
            for (int i = 0; i < DBFoodManager.FOOD_COUNT; i++)
            {
                for (int j = 0; j < totalRewardsCount; j++)
                {
                    int foodIndex = i;
                    int rewardIndex = j;
                    AddButtonClickListener(foodProgressComponents.AchievementMakeFoodClaimButton[(i * totalRewardsCount) + j], foodIndex, rewardIndex, true);
                    AddButtonClickListener(hireProgressComponents.AchievementHireBakerClaimButton[(i * totalRewardsCount) + j], foodIndex, rewardIndex, false);
                }
            }
        }

        private void AddButtonClickListener(Button button, int foodIndex, int rewardIndex, bool isMakeFoodReward) =>
            button.onClick.AddListener(() => ClaimReward(foodIndex, rewardIndex, isMakeFoodReward));
        
        private void ClaimReward(int foodIndex, int rewardIndex, bool isMakeFoodReward)
        {
            OnProgressCompletion(rewardIndex);
            if (isMakeFoodReward)
            {
                ClaimMakeFoodReward(rewardIndex, foodIndex);
            }
            else
            {
                ClaimHireBakerReward(rewardIndex, foodIndex);
            }
        }

        private void UpdateSliderValue(Slider progressSlider, int progress, int goal)
        {
            float targetValue = (float)progress / goal;
            progressSlider.value = targetValue;
        }

        public void UpdateProgress(int foodIndex, int goal, bool isMakeFood)
        {
            int progress = isMakeFood ? GetFoodData(foodIndex).FoodCookedCount : GetFoodData(foodIndex).BakersCount;
            Slider progressSlider = isMakeFood ? foodProgressComponents.AchievementsMakeFoodProgressSlider[foodIndex] : hireProgressComponents.AchievementsHireBakerProgressSlider[foodIndex];
            TMP_Text progressText = isMakeFood ? foodProgressComponents.AchievementMakeFoodCountText[foodIndex] : hireProgressComponents.AchievementHireBakerCountText[foodIndex];

            progressText.text = $"{progress} / {goal}";
            UpdateSliderValue(progressSlider, progress, goal);
        }

        public void UpdateMakeFoodProgress(int foodIndex, int goal)
        {
            UpdateProgress(foodIndex, goal, true);
        }

        public void UpdateHireBakerProgress(int foodIndex, int goal)
        {
            UpdateProgress(foodIndex, goal, false);
        }

        public void UpdateClaimButtonStatus(int foodIndex, int currentReward, bool isMakeFood, bool value)
        {
            int buttonIndex = GetButtonIndex(foodIndex, currentReward);
            if (isMakeFood)
            {
                foodProgressComponents.AchievementMakeFoodClaimButton[buttonIndex].gameObject.SetActive(value);
            }
            else
            {
                hireProgressComponents.AchievementHireBakerClaimButton[buttonIndex].gameObject.SetActive(value);
            }
        }
   
        private void OnProgressCompletion(int currentMilestone)
        {
            double reward = DBAchievementsController.FoodItemsAchievementsRewards[currentMilestone];
            InvokeEvent(DBEventNames.AddStarsUpdate, reward);
        }

        private void ClaimMakeFoodReward(int rewardIndex, int foodIndex)
        {
            for (int i = 0; i <= rewardIndex; i++)
            {
                UpdateClaimButtonStatus(foodIndex, rewardIndex, true, false);
            }

            achievementsController.AchievementClaimReward.IsMakeFoodRewardClaimed[foodIndex, rewardIndex] = true;
            InvokeEvent(DBEventNames.MakeFoodProgressUpdate, foodIndex);
            InvokeEvent(DBEventNames.CheckBuySkinButtonUI, null);
            InvokeEvent(DBEventNames.CheckBuyTimeWrapButtonsUI, null);
            claimData.SaveAchievementClaims();
        }

        private void ClaimHireBakerReward(int rewardIndex, int foodIndex)
        {
            for (int i = 0; i <= rewardIndex; i++)
            {
                UpdateClaimButtonStatus(foodIndex, rewardIndex, false, false);
            }

            achievementsController.AchievementClaimReward.IsHireBakerRewardClaimed[foodIndex, rewardIndex] = true;
            InvokeEvent(DBEventNames.HireBakerProgressUpdate, foodIndex);
            InvokeEvent(DBEventNames.CheckBuySkinButtonUI, null);
            InvokeEvent(DBEventNames.CheckBuyTimeWrapButtonsUI, null);
            claimData.SaveAchievementClaims();
        }

        private int GetButtonIndex(int foodIndex, int rewardIndex)
        {
            int totalRewards = DBAchievementsController.FoodItemsAchievementsRewards.Length;
            int buttonIndex = (foodIndex * totalRewards) + rewardIndex;
            return buttonIndex;
        }
    }

    [Serializable]
    public class AchievementsMakeFoodProgressComponents
    {
        public Slider[] AchievementsMakeFoodProgressSlider;
        public TMP_Text[] AchievementMakeFoodCountText;
        public Button[] AchievementMakeFoodClaimButton;
    }

    [Serializable]
    public class AchievementsHireBakerProgressComponents
    {
        public Slider[] AchievementsHireBakerProgressSlider;
        public TMP_Text[] AchievementHireBakerCountText;
        public Button[] AchievementHireBakerClaimButton;
    }
}