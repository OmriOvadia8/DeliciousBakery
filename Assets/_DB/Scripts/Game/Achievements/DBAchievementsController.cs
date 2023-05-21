using UnityEngine;
using DB_Core;
using System;

namespace DB_Game
{
    // DBAchievementsController handles the logic for checking and updating achievements related to cooking and hiring bakers.
    public class DBAchievementsController : FoodDataAccess
    {
        [SerializeField] DBAchievementsDataManager achievementsManager;
        public AchievementData Achievements => achievementsManager.AchievementsData.Achievements;
        public AchievementClaimData AchievementClaimReward => achievementsManager.AchievementClaimData;
        
        public static readonly int[] FoodItemAchievementGoals = { 10, 100, 500, 1000, 5000 };
        public static readonly int[] FoodItemsAchievementsRewards = { 100, 200, 300, 500, 1000 };

        private void OnEnable() => RegisterEvents();

        private void OnDisable() => UnregisterEvents();

        // This region's methods check if the player has reached any cooking/hired baker achievement milestones
        // for a specific food item, based on its current count.
        // It sets the corresponding achievement property to true if the milestone is reached.
        #region individual Cooked/Hired Achievement
        private void CheckCookedFoodAchievements(object foodIndex)
        {
            int index = (int)foodIndex;
            int currentFoodCount = GetFoodData(index).FoodCookedCount;

            Action<int>[] setAchievement = {
                index => Achievements.Cook10[index] = true,
                index => Achievements.Cook100[index] = true,
                index => Achievements.Cook500[index] = true,
                index => Achievements.Cook1000[index] = true,
                index => Achievements.Cook5000[index] = true
                                                            };

            for (int i = 0; i < FoodItemAchievementGoals.Length; i++)
            {
                if (currentFoodCount >= FoodItemAchievementGoals[i])
                {
                    setAchievement[i](index);
                    achievementsManager.SaveAchievementsData();
                }
            }
        }

        private void CheckHiredBakersAchievements(object foodIndex)
        {
            int index = (int)foodIndex;

            int currentBakersCount = GetFoodData(index).BakersCount;

            Action<int>[] setAchievement = {
                index => Achievements.Hire10[index] = true,
                index => Achievements.Hire100[index] = true,
                index => Achievements.Hire500[index] = true,
                index => Achievements.Hire1000[index] = true,
                index => Achievements.Hire5000[index] = true
                                                            };

            for (int i = 0; i < FoodItemAchievementGoals.Length; i++)
            {
                if (currentBakersCount >= FoodItemAchievementGoals[i])
                {
                    setAchievement[i](index);
                    achievementsManager.SaveAchievementsData();
                }
            }
        }
        #endregion

        private void RegisterEvents()
        {
            AddListener(DBEventNames.CheckCookedAchievement, CheckCookedFoodAchievements);
            AddListener(DBEventNames.CheckHiredAchievement, CheckHiredBakersAchievements);
        }

        private void UnregisterEvents()
        {
            RemoveListener(DBEventNames.CheckCookedAchievement, CheckCookedFoodAchievements);
            RemoveListener(DBEventNames.CheckHiredAchievement, CheckHiredBakersAchievements);
        }
    }
}