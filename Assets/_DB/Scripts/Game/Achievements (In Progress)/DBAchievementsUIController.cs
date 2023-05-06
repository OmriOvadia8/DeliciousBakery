using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DB_Core;

namespace DB_Game
{
    public class DBAchievementsUIController : DBLogicMonoBehaviour
    {
        [Header("Achievements UI Components")]
        [SerializeField] AchievementMakeFoodComponents makeFoodAchievementComponents;
        [SerializeField] AchievementHireBakerComponents hireBakerAchievementComponents;
        [SerializeField] DBAchievementsController achievementController;
        [SerializeField] AchievementsDOTweenController achievementsDOTweenController;
        private AchievementTextProvider achievementsTextProvider;

        private void OnEnable()
        {
            achievementsTextProvider = new AchievementTextProvider(DBFoodManager.FOOD_COUNT);
            AddListener(DBEventNames.CurrentMakeFoodAchievementStatus, UpdateMakeFoodAchievementsText);
            AddListener(DBEventNames.CurrentHireBakerAchievementStatus, UpdateHireBakerAchievementsText);
        }

        private void OnDisable()
        {
            RemoveListener(DBEventNames.CurrentMakeFoodAchievementStatus, UpdateMakeFoodAchievementsText);
            RemoveListener(DBEventNames.CurrentHireBakerAchievementStatus, UpdateHireBakerAchievementsText);
        }

        private void Start()
        {
            InitializeCurrentAchievementsState();
        }

        private void UpdateMakeFoodAchievementsText(object index)
        {
            int foodIndex = (int)index;
            FoodTypes foodName = (FoodTypes)foodIndex;
            CookMilestoneIndex currentMilestoneIndex = DetermineCurrentCookMilestone(foodName);
            UpdateMakeFoodAchievementTitle(foodIndex, foodName, currentMilestoneIndex);
            CheckIfMakeFoodAchievementCompleted(foodIndex);
            int goal = GetCurrentFoodAchievementGoal(currentMilestoneIndex);
            ClaimRewardActivation(foodIndex);
            achievementsDOTweenController.UpdateMakeFoodProgress(foodIndex, goal);
            UpdateMakeFoodAchievementDescription(goal, foodIndex, foodName);
            UpdateMakeFoodStars(foodIndex, currentMilestoneIndex);
            
        }

        private void UpdateMakeFoodAchievementTitle(int foodIndex , FoodTypes foodName, CookMilestoneIndex currentMilestoneIndex)
        {
            int milestoneIndex = (int)currentMilestoneIndex;
            var makeFoodTitleText = achievementsTextProvider.GetMakeFoodTitle(foodName, milestoneIndex);
            var currentMakeFoodTitle = makeFoodAchievementComponents.AchievementMakeFoodTitle[foodIndex];

            currentMakeFoodTitle.text = makeFoodTitleText;
        }

        private void UpdateMakeFoodAchievementDescription(int goal, int foodIndex, FoodTypes foodName)
        {
            var makeFoodDescriptionText = achievementsTextProvider.GetMakeFoodDescription(foodName, goal);
            var currentMakeFoodDescription = makeFoodAchievementComponents.AchievementMakeFoodDescription[foodIndex];

            currentMakeFoodDescription.text = makeFoodDescriptionText;
        }

        private int GetCurrentFoodAchievementGoal(CookMilestoneIndex currentMilestoneIndex)
        {
            int indexToAccess = (int)currentMilestoneIndex;
            return GetCurrentAchievementGoal(indexToAccess);
        }

        private void UpdateMakeFoodStars(int foodIndex, CookMilestoneIndex currentMilestoneIndex)
        {
            int currentMilestone = (int)currentMilestoneIndex;
            Image[] stars = makeFoodAchievementComponents.AchievementMakeFoodStars[foodIndex].Stars;

            for (int i = 0; i < stars.Length; i++)
            {
                stars[i].color = Color.gray;

                if (i <= currentMilestone)
                {
                    stars[i].color = Color.white;
                }
            }
        }

        private void UpdateHireBakerAchievementsText(object index)
        {
            int foodIndex = (int)index;
            FoodTypes foodName = (FoodTypes)foodIndex;
            HireMilestoneIndex currentMilestoneIndex = DetermineCompletedHireMilestone(foodName);
            UpdateHireBakerAchievementTitle(foodIndex, foodName, currentMilestoneIndex);
            CheckIfHireBakerAchievementCompleted(foodIndex);
            int goal = GetCurrentHireBakerGoal(currentMilestoneIndex);
            achievementsDOTweenController.UpdateHireBakerProgress(foodIndex, goal);
            UpdateHireBakerStars(foodIndex, currentMilestoneIndex);
            UpdateHireBakerAchievementDescription(goal, foodIndex, foodName);
            ClaimRewardActivation(foodIndex);
        }

        private void UpdateHireBakerAchievementTitle(int foodIndex, FoodTypes foodName, HireMilestoneIndex completedMilestoneIndex)
        {
            int milestoneIndex = (int)completedMilestoneIndex;
            var hireBakerTitleText = achievementsTextProvider.GetHireBakerTitle(foodName, milestoneIndex);
            var currentHireBakerTitle = hireBakerAchievementComponents.AchievementHireBakerTitle[foodIndex];

            currentHireBakerTitle.text = hireBakerTitleText;
        }

        private void UpdateHireBakerAchievementDescription(int goal, int foodIndex, FoodTypes foodName)
        {
            var hireBakerDescriptionText = achievementsTextProvider.GetHireBakerDescription(foodName, goal);
            var currentHireBakerDescription = hireBakerAchievementComponents.AchievementHireBakerDescription[foodIndex];

            currentHireBakerDescription.text = hireBakerDescriptionText;
        }

        private void UpdateHireBakerStars(int foodIndex, HireMilestoneIndex currentMilestoneIndex)
        {
            int currentMilestone = (int)currentMilestoneIndex;
            Image[] stars = hireBakerAchievementComponents.AchievementHireBakerStars[foodIndex].Stars;

            for (int i = 0; i < stars.Length; i++)
            {
                stars[i].color = Color.gray;

                if (i <= currentMilestone)
                {
                    stars[i].color = Color.white;
                }
            }
        }

        private int GetCurrentHireBakerGoal(HireMilestoneIndex currentMilestoneIndex)
        {
            int indexToAccess = (int)currentMilestoneIndex;
            return GetCurrentAchievementGoal(indexToAccess);
        }

        private void InitializeCurrentAchievementsState()
        {
            for (int i = 0; i < DBFoodManager.FOOD_COUNT; i++)
            {
                UpdateMakeFoodAchievementsText(i);
                UpdateHireBakerAchievementsText(i);
            }
        }

        private void CheckIfMakeFoodAchievementCompleted(int foodIndex)
        {
            if (achievementController.Achievements.Cook5000[foodIndex])
            {
                makeFoodAchievementComponents.AchievementMakeFoodCompleteBar[foodIndex].SetActive(true);
                makeFoodAchievementComponents.AchievementMakeFoodLoadProgressBar[foodIndex].SetActive(false);
                makeFoodAchievementComponents.AchievementMakeFoodDescription[foodIndex].gameObject.SetActive(false);
                makeFoodAchievementComponents.AchievementMakeFoodTitle[foodIndex].gameObject.SetActive(false);

                Image[] stars = makeFoodAchievementComponents.AchievementMakeFoodStars[foodIndex].Stars;
                for (int i = 0; i < stars.Length; i++)
                {
                    stars[i].color = Color.white;
                }
            }
        }

        private void CheckIfHireBakerAchievementCompleted(int foodIndex)
        {
            if (achievementController.Achievements.Hire5000[foodIndex])
            {
                hireBakerAchievementComponents.AchievementHireBakerCompleteBar[foodIndex].SetActive(true);
                hireBakerAchievementComponents.AchievementHireBakerLoadProgressBar[foodIndex].SetActive(false);
                hireBakerAchievementComponents.AchievementHireBakerDescription[foodIndex].gameObject.SetActive(false);
                hireBakerAchievementComponents.AchievementHireBakerTitle[foodIndex].gameObject.SetActive(false);
                
                Image[] stars = hireBakerAchievementComponents.AchievementHireBakerStars[foodIndex].Stars;
                for (int i = 0; i < stars.Length; i++)
                {
                    stars[i].color = Color.white;
                }
            }
        }

        private CookMilestoneIndex DetermineCurrentCookMilestone(FoodTypes foodName)
        {
            CookMilestoneIndex currentCookMilestone = CookMilestoneIndex.Cook10;
            int foodIndex = (int)foodName;

            if (achievementController.Achievements.Cook1000[foodIndex])
            {
                
                currentCookMilestone = CookMilestoneIndex.Cook5000;
            }
            else if (achievementController.Achievements.Cook500[foodIndex])
            {
                currentCookMilestone = CookMilestoneIndex.Cook1000;
            }
            else if (achievementController.Achievements.Cook100[foodIndex])
            {
                currentCookMilestone = CookMilestoneIndex.Cook500;
            }
            else if (achievementController.Achievements.Cook10[foodIndex])
            {
                currentCookMilestone = CookMilestoneIndex.Cook100;
            }

            return currentCookMilestone;
        }

        private HireMilestoneIndex DetermineCompletedHireMilestone(FoodTypes foodName)
        {
            HireMilestoneIndex currentHireMilestone = HireMilestoneIndex.Hire10;
            int foodIndex = (int)foodName;

            if (achievementController.Achievements.Hire1000[foodIndex])
            {
                currentHireMilestone = HireMilestoneIndex.Hire5000;
            }
            else if (achievementController.Achievements.Hire500[foodIndex])
            {
                currentHireMilestone = HireMilestoneIndex.Hire1000;
            }
            else if (achievementController.Achievements.Hire100[foodIndex])
            {
                currentHireMilestone = HireMilestoneIndex.Hire500;
            }
            else if(achievementController.Achievements.Hire10[foodIndex])
            {
                currentHireMilestone = HireMilestoneIndex.Hire100;
            }

            return currentHireMilestone;
        }

        private void ClaimRewardActivation(int foodIndex)
        {
            if(!achievementController.AchievementClaimReward.IsMakeFoodRewardClaimed[foodIndex, 0] && achievementController.Achievements.Cook10[foodIndex])
            {
                achievementsDOTweenController.MakeFoodClaimButtonStatus(foodIndex, 0, true);
            }

            if (!achievementController.AchievementClaimReward.IsMakeFoodRewardClaimed[foodIndex, 1] && achievementController.Achievements.Cook100[foodIndex])
            {
                achievementsDOTweenController.MakeFoodClaimButtonStatus(foodIndex, 1, true);
            }

            if (!achievementController.AchievementClaimReward.IsMakeFoodRewardClaimed[foodIndex, 2] && achievementController.Achievements.Cook500[foodIndex])
            {
                achievementsDOTweenController.MakeFoodClaimButtonStatus(foodIndex, 2, true);
            }

            if (!achievementController.AchievementClaimReward.IsMakeFoodRewardClaimed[foodIndex, 3] && achievementController.Achievements.Cook1000[foodIndex])
            {
                achievementsDOTweenController.MakeFoodClaimButtonStatus(foodIndex, 3, true);
            }

            if (!achievementController.AchievementClaimReward.IsMakeFoodRewardClaimed[foodIndex, 4] && achievementController.Achievements.Cook5000[foodIndex])
            {
                achievementsDOTweenController.MakeFoodClaimButtonStatus(foodIndex, 4, true);
            }

            if (!achievementController.AchievementClaimReward.IsHireBakerRewardClaimed[foodIndex, 0] && achievementController.Achievements.Hire10[foodIndex])
            {
                achievementsDOTweenController.HireBakerClaimButtonStatus(foodIndex, 0, true);
            }

            if (!achievementController.AchievementClaimReward.IsHireBakerRewardClaimed[foodIndex, 1] && achievementController.Achievements.Hire100[foodIndex])
            {
                achievementsDOTweenController.HireBakerClaimButtonStatus(foodIndex, 1, true);
            }

            if (!achievementController.AchievementClaimReward.IsHireBakerRewardClaimed[foodIndex, 2] && achievementController.Achievements.Hire500[foodIndex])
            {
                achievementsDOTweenController.HireBakerClaimButtonStatus(foodIndex, 2, true);
            }

            if (!achievementController.AchievementClaimReward.IsHireBakerRewardClaimed[foodIndex, 3] && achievementController.Achievements.Hire1000[foodIndex])
            {
                achievementsDOTweenController.HireBakerClaimButtonStatus(foodIndex, 3, true);
            }

            if (!achievementController.AchievementClaimReward.IsHireBakerRewardClaimed[foodIndex, 4] && achievementController.Achievements.Hire5000[foodIndex])
            {
                achievementsDOTweenController.HireBakerClaimButtonStatus(foodIndex, 4, true);
            }
        }

      

        private int GetCurrentAchievementGoal(int index)
        {
            if (index >= DBAchievementsController.FoodItemAchievementGoals.Length)
            {
            index = DBAchievementsController.FoodItemAchievementGoals.Length - 1;
            }
            int goal = DBAchievementsController.FoodItemAchievementGoals[index];

            return goal;
        }
    }

    [Serializable]
    public class AchievementMakeFoodComponents
    {
        public TMP_Text[] AchievementMakeFoodTitle;
        public TMP_Text[] AchievementMakeFoodDescription;
        public FoodStars[] AchievementMakeFoodStars;
        public GameObject[] AchievementMakeFoodCompleteBar;
        public GameObject[] AchievementMakeFoodLoadProgressBar;
    }

    [Serializable]
    public class AchievementHireBakerComponents
    {
        public TMP_Text[] AchievementHireBakerTitle;
        public TMP_Text[] AchievementHireBakerDescription;
        public HireStars[] AchievementHireBakerStars;
        public GameObject[] AchievementHireBakerCompleteBar;
        public GameObject[] AchievementHireBakerLoadProgressBar;
    }
    
    [Serializable]
    public class FoodStars
    {
        public Image[] Stars;
    }

    [Serializable]
    public class HireStars
    {
        public Image[] Stars;
    }

    public enum CookMilestoneIndex
    {
        Cook10 = 0,
        Cook100 = 1,
        Cook500 = 2,
        Cook1000 = 3,
        Cook5000 = 4,
    }

    public enum HireMilestoneIndex
    {
        Hire10 = 0,
        Hire100 = 1,
        Hire500 = 2,
        Hire1000 = 3,
        Hire5000 = 4
    }

}