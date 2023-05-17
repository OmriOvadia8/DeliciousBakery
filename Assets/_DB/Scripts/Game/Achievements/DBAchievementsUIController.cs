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
        [Header("Achievements Controllers")]
        [SerializeField] DBAchievementsController achievementController;
        [SerializeField] DBAchievementsProgressUIController progressController;

        private AchievementTextProvider achievementsTextProvider;
        private readonly CookMilestoneIndex[] cookMilestones = { CookMilestoneIndex.Cook5000, CookMilestoneIndex.Cook1000, CookMilestoneIndex.Cook500, CookMilestoneIndex.Cook100, CookMilestoneIndex.Cook10 };
        private readonly HireMilestoneIndex[] hireMilestones = { HireMilestoneIndex.Hire5000, HireMilestoneIndex.Hire1000, HireMilestoneIndex.Hire500, HireMilestoneIndex.Hire100, HireMilestoneIndex.Hire10 };

        private void OnEnable() => InitializeTextProviderAndRegisterEvents();

        private void OnDisable() => UnregisterEvents();

        private void Start() => InitializeCurrentAchievementsState();

        private void UpdateMakeFoodAchievementsProgress(object index)
        {
            int foodIndex = (int)index;
            FoodTypes foodName = (FoodTypes)foodIndex;
            CookMilestoneIndex currentMilestoneIndex = DetermineCurrentCookMilestone(foodName);
            CheckIfMakeFoodAchievementCompleted(foodIndex);
            int goal = GetCurrentFoodAchievementGoal(currentMilestoneIndex);
            ClaimRewardActivation(foodIndex);
            progressController.UpdateMakeFoodProgress(foodIndex, goal);
        }

        private void UpdateMakeFoodAchievementsText(object index)
        {
            int foodIndex = (int)index;
            FoodTypes foodName = (FoodTypes)foodIndex;
            CookMilestoneIndex currentMilestoneIndex = DetermineCurrentCookMilestone(foodName);

            UpdateMakeFoodAchievementTitle(foodIndex, foodName, currentMilestoneIndex);
            int goal = GetCurrentFoodAchievementGoal(currentMilestoneIndex);
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

        private void UpdateHireBakerAchievementsText(object index)
        {
            int foodIndex = (int)index;
            FoodTypes foodName = (FoodTypes)foodIndex;
            HireMilestoneIndex currentMilestoneIndex = DetermineCurrentHireMilestone(foodName);

            UpdateHireBakerAchievementTitle(foodIndex, foodName, currentMilestoneIndex);
            int goal = GetCurrentHireBakerGoal(currentMilestoneIndex);
            UpdateHireBakerAchievementDescription(goal, foodIndex, foodName);
            UpdateHireBakerStars(foodIndex, currentMilestoneIndex);
        }

        private void UpdateHireBakerAchievementsProgress(object index)
        {
            int foodIndex = (int)index;
            FoodTypes foodName = (FoodTypes)foodIndex;
            HireMilestoneIndex currentMilestoneIndex = DetermineCurrentHireMilestone(foodName);
            CheckIfHireBakerAchievementCompleted(foodIndex);
            int goal = GetCurrentHireBakerGoal(currentMilestoneIndex);
            ClaimRewardActivation(foodIndex);
            progressController.UpdateHireBakerProgress(foodIndex, goal);
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

        private void UpdateMakeFoodStars(int foodIndex, CookMilestoneIndex currentMilestoneIndex)
        {
            Image[] stars = makeFoodAchievementComponents.AchievementMakeFoodStars[foodIndex].Stars;
            UpdateStars(stars, (int)currentMilestoneIndex);
        }

        private void UpdateHireBakerStars(int foodIndex, HireMilestoneIndex currentMilestoneIndex)
        {
            Image[] stars = hireBakerAchievementComponents.AchievementHireBakerStars[foodIndex].Stars;
            UpdateStars(stars, (int)currentMilestoneIndex);
        }

        private void UpdateStars(Image[] stars, int currentMilestone)
        {
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
                UpdateMakeFoodAchievementsProgress(i);
                UpdateHireBakerAchievementsProgress(i);
            }

            InitializeCurrentMakeFoodAchievementsText();
            InitializeCurrentHireBakerAchievementText();


        }

        private void InitializeCurrentMakeFoodAchievementsText()
        {
            for (int foodIndex = 0; foodIndex < Enum.GetValues(typeof(FoodTypes)).Length; foodIndex++)
            {
                FoodTypes foodName = (FoodTypes)foodIndex;
                CookMilestoneIndex currentMilestoneIndex = DetermineCurrentCookMilestone(foodName);
                UpdateMakeFoodAchievementTitle(foodIndex, foodName, currentMilestoneIndex);
                int goal = GetCurrentFoodAchievementGoal(currentMilestoneIndex);
                UpdateMakeFoodAchievementDescription(goal, foodIndex, foodName);
                UpdateMakeFoodStars(foodIndex, currentMilestoneIndex);
            }
        }

        private void InitializeCurrentHireBakerAchievementText()
        {
            for (int foodIndex = 0; foodIndex < Enum.GetValues(typeof(FoodTypes)).Length; foodIndex++)
            {
                FoodTypes foodName = (FoodTypes)foodIndex;
                HireMilestoneIndex currentMilestoneIndex = DetermineCurrentHireMilestone(foodName);
                UpdateHireBakerAchievementTitle(foodIndex, foodName, currentMilestoneIndex);
                int goal = GetCurrentHireBakerGoal(currentMilestoneIndex);
                UpdateHireBakerAchievementDescription(goal, foodIndex, foodName);
                UpdateHireBakerStars(foodIndex, currentMilestoneIndex);
            }
        }

        private void CheckIfAchievementCompleted(int foodIndex, bool isMakeFood, bool isCompleted)
        {
            if (isCompleted)
            {
                if (isMakeFood)
                {
                    MakeFoodAchievementCompleted(foodIndex);
                }
                else
                {
                    HireBakerAchievementCompleted(foodIndex);
                }
            }
        }

        private void MakeFoodAchievementCompleted(int foodIndex)
        {
            makeFoodAchievementComponents.AchievementMakeFoodCompleteBar[foodIndex].SetActive(true);
            makeFoodAchievementComponents.AchievementMakeFoodLoadProgressBar[foodIndex].SetActive(false);
            makeFoodAchievementComponents.AchievementMakeFoodDescription[foodIndex].gameObject.SetActive(false);

            Image[] stars = makeFoodAchievementComponents.AchievementMakeFoodStars[foodIndex].Stars;
            UpdateStars(stars, (int)CookMilestoneIndex.Cook5000);
        }

        private void HireBakerAchievementCompleted(int foodIndex)
        {
            hireBakerAchievementComponents.AchievementHireBakerCompleteBar[foodIndex].SetActive(true);
            hireBakerAchievementComponents.AchievementHireBakerLoadProgressBar[foodIndex].SetActive(false);
            hireBakerAchievementComponents.AchievementHireBakerDescription[foodIndex].gameObject.SetActive(false);

            Image[] stars = hireBakerAchievementComponents.AchievementHireBakerStars[foodIndex].Stars;
            UpdateStars(stars, (int)HireMilestoneIndex.Hire5000);
        }


        private void CheckIfMakeFoodAchievementCompleted(int foodIndex)
        {
            CheckIfAchievementCompleted(foodIndex, true, achievementController.Achievements.Cook5000[foodIndex]);
        }

        private void CheckIfHireBakerAchievementCompleted(int foodIndex)
        {
            CheckIfAchievementCompleted(foodIndex, false, achievementController.Achievements.Hire5000[foodIndex]);
        }

        private CookMilestoneIndex DetermineCurrentCookMilestone(FoodTypes foodName)
        {
            int foodIndex = (int)foodName;
            bool[][] achievements = { achievementController.Achievements.Cook1000, achievementController.Achievements.Cook500, achievementController.Achievements.Cook100, achievementController.Achievements.Cook10 };

            for (int i = 0; i < achievements.Length; i++)
            {
                if (achievements[i][foodIndex])
                {
                    return cookMilestones[i];
                }
            }

            return CookMilestoneIndex.Cook10;
        }

        private HireMilestoneIndex DetermineCurrentHireMilestone(FoodTypes foodName)
        {
            int foodIndex = (int)foodName;
            bool[][] achievements = { achievementController.Achievements.Hire1000, achievementController.Achievements.Hire500, achievementController.Achievements.Hire100, achievementController.Achievements.Hire10 };

            for (int i = 0; i < achievements.Length; i++)
            {
                if (achievements[i][foodIndex])
                {
                    return hireMilestones[i];
                }
            }

            return HireMilestoneIndex.Hire10;
        }

        private void ClaimRewardActivation(int foodIndex)
        {
            bool[][] makeFoodAchievements = { achievementController.Achievements.Cook10, achievementController.Achievements.Cook100, achievementController.Achievements.Cook500,
                                                achievementController.Achievements.Cook1000, achievementController.Achievements.Cook5000 };
            bool[][] hireBakerAchievements = { achievementController.Achievements.Hire10, achievementController.Achievements.Hire100, achievementController.Achievements.Hire500,
                                                achievementController.Achievements.Hire1000, achievementController.Achievements.Hire5000 };

            for (int i = 0; i < makeFoodAchievements.Length; i++)
            {
                if (!achievementController.AchievementClaimReward.IsMakeFoodRewardClaimed[foodIndex, i] && makeFoodAchievements[i][foodIndex])
                {
                    progressController.UpdateClaimButtonStatus(foodIndex, i, true, true);
                }

                if (!achievementController.AchievementClaimReward.IsHireBakerRewardClaimed[foodIndex, i] && hireBakerAchievements[i][foodIndex])
                {
                    progressController.UpdateClaimButtonStatus(foodIndex, i, false, true);
                }
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

        private void InitializeTextProviderAndRegisterEvents()
        {
            achievementsTextProvider ??= new AchievementTextProvider(DBFoodManager.FOOD_COUNT);

            AddListener(DBEventNames.CurrentMakeFoodAchievementStatus, UpdateMakeFoodAchievementsProgress);
            AddListener(DBEventNames.CurrentHireBakerAchievementStatus, UpdateHireBakerAchievementsProgress);
            AddListener(DBEventNames.MakeFoodProgressUpdate, UpdateMakeFoodAchievementsText);
            AddListener(DBEventNames.HireBakerProgressUpdate, UpdateHireBakerAchievementsText);
        }

        private void UnregisterEvents()
        {
            RemoveListener(DBEventNames.CurrentMakeFoodAchievementStatus, UpdateMakeFoodAchievementsProgress);
            RemoveListener(DBEventNames.CurrentHireBakerAchievementStatus, UpdateHireBakerAchievementsProgress);
            RemoveListener(DBEventNames.MakeFoodProgressUpdate, UpdateMakeFoodAchievementsText);
            RemoveListener(DBEventNames.HireBakerProgressUpdate, UpdateHireBakerAchievementsText);
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