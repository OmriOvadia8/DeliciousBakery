using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using DB_Core;
using DG.Tweening;

namespace DB_Game
{
    public class AchievementsDOTweenController : FoodDataAccess
    {
        [SerializeField] AchievementsDOTweenMakeFoodComponents achievementsFoodDOTween;
        [SerializeField] AchievementsDOTweenHireBakerComponents achievementsHireDOTween;
        [SerializeField] DBAchievementsController achievementsController;

        private void Start()
        {
            for (int i = 0; i < DBFoodManager.FOOD_COUNT; i++)
            {
                for (int j = 0; j < DBAchievementsController.FoodItemsAchievementsRewards.Length; j++)
                {
                    int foodIndex = i;
                    int rewardIndex = j;
                    achievementsFoodDOTween.AchievementMakeFoodClaimButton[i * 5 + j].onClick.AddListener(() => ClaimReward(foodIndex, rewardIndex, true));
                    achievementsHireDOTween.AchievementHireBakerClaimButton[i * 5 + j].onClick.AddListener(() => ClaimReward(foodIndex, rewardIndex, false));
                }
            }
        }

        public void ClaimReward(int foodIndex, int rewardIndex, bool isMakeFoodReward)
        {
            OnProgressCompletion(rewardIndex);
            if (isMakeFoodReward)
            {
                for (int i = 0; i <= rewardIndex; i++)
                {
                    MakeFoodClaimButtonStatus(foodIndex, i, false);
                }
                // Mark the reward as claimed
                achievementsController.AchievementClaimReward.IsMakeFoodRewardClaimed[foodIndex, rewardIndex] = true;
            }
            else
            {
                for (int i = 0; i <= rewardIndex; i++)
                {
                    HireBakerClaimButtonStatus(foodIndex, i, false);
                }
                // Mark the reward as claimed
                achievementsController.AchievementClaimReward.IsHireBakerRewardClaimed[foodIndex, rewardIndex] = true;
            }
        }

        public void UpdateProgress(Slider progressSlider,int progress, int goal)
        {
            float targetValue = (float)progress / goal;
            progressSlider.DOValue(targetValue, 0).SetEase(Ease.Linear);
        }

        public void UpdateMakeFoodProgress(int foodIndex, int goal)
        {
            int makeFoodProgress = GetFoodData(foodIndex).FoodCookedCount;
            Slider progressSlider = achievementsFoodDOTween.AchievementsMakeFoodProgressSlider[foodIndex];
            achievementsFoodDOTween.AchievementMakeFoodCountText[foodIndex].text = $"{makeFoodProgress} / {goal}";

            UpdateProgress(progressSlider, makeFoodProgress, goal);
        }

        public void UpdateHireBakerProgress(int foodIndex, int goal)
        {
            int hireBakerProgress = GetFoodData(foodIndex).BakersCount;
            Slider progressSlider = achievementsHireDOTween.AchievementsHireBakerProgressSlider[foodIndex];
            achievementsHireDOTween.AchievementHireBakerCountText[foodIndex].text = $"{hireBakerProgress} / {goal}";
            UpdateProgress(progressSlider, hireBakerProgress, goal);
        }

        public void MakeFoodClaimButtonStatus(int foodIndex, int currentReward, bool value)
        {
            int buttonIndex = GetButtonIndex(foodIndex, currentReward);
            achievementsFoodDOTween.AchievementMakeFoodClaimButton[buttonIndex].gameObject.SetActive(value);
        }

        public void HireBakerClaimButtonStatus(int foodIndex, int currentReward, bool value)
        {
            int buttonIndex = GetButtonIndex(foodIndex, currentReward);
            achievementsHireDOTween.AchievementHireBakerClaimButton[buttonIndex].gameObject.SetActive(value);
        }

        private int GetButtonIndex(int foodIndex, int rewardIndex)
        {
            int totalRewards = DBAchievementsController.FoodItemsAchievementsRewards.Length; 
            int buttonIndex = foodIndex * totalRewards + rewardIndex;
            return buttonIndex;
        }
        public void OnProgressCompletion(int currentMilestone)
        {
            int reward = DBAchievementsController.FoodItemsAchievementsRewards[currentMilestone];
            InvokeEvent(DBEventNames.AddStarsUpdate, reward);
        }

        public void ContinueMakeFoodProgress(Button[] claimButton, int foodIndex)
        {
            claimButton[foodIndex].gameObject.SetActive(false);
            InvokeEvent(DBEventNames.CurrentMakeFoodAchievementStatus, foodIndex);
        }

        public void ContinueHireBakerProgress(Button[] claimButton, int foodIndex)
        {
            claimButton[foodIndex].gameObject.SetActive(false);
            InvokeEvent(DBEventNames.CurrentHireBakerAchievementStatus, foodIndex);
        }

        public void FirstRewardComplete()
        {

        }
    }

    [Serializable]
    public class AchievementsDOTweenMakeFoodComponents
    {
        public Slider[] AchievementsMakeFoodProgressSlider;
        public TMP_Text[] AchievementMakeFoodCountText;
        public Button[] AchievementMakeFoodClaimButton;
    }

    [Serializable]
    public class AchievementsDOTweenHireBakerComponents
    {
        public Slider[] AchievementsHireBakerProgressSlider;
        public TMP_Text[] AchievementHireBakerCountText;
        public Button[] AchievementHireBakerClaimButton;
    }


}