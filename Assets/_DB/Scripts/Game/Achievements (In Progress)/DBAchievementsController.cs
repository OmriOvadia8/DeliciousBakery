using UnityEngine;
using DB_Core;
using System;

namespace DB_Game
{
    // DBAchievementsController handles the logic for checking and updating achievements related to cooking and hiring bakers.
    public class DBAchievementsController : FoodDataAccess
    {
        [SerializeField] DBAchievementsManager achievementsManager;

        private AchievementData Achievements => achievementsManager.AchievementsData.Achievements;
        
        public const int ACHIEVEMENT_COUNT_10 = 1;
        public const int ACHIEVEMENT_COUNT_100 = 100;
        public const int ACHIEVEMENT_COUNT_500 = 500;
        public const int ACHIEVEMENT_COUNT_1000 = 1000;
        public const int ACHIEVEMENT_COUNT_5000 = 5000;

        public const int ACHIEVEMENT_TOTAL_100 = 3;
        public const int ACHIEVEMENT_TOTAL_1000 = 1000;
        public const int ACHIEVEMENT_TOTAL_5000 = 5000;
        public const int ACHIEVEMENT_TOTAL_10000 = 10000;
        public const int ACHIEVEMENT_TOTAL_20000 = 20000;

        private readonly int[] individualAchievements =
            { ACHIEVEMENT_COUNT_10, ACHIEVEMENT_COUNT_100, ACHIEVEMENT_COUNT_500, ACHIEVEMENT_COUNT_1000, ACHIEVEMENT_COUNT_5000 };

        private readonly int[] totalAchievements =
            { ACHIEVEMENT_TOTAL_100, ACHIEVEMENT_TOTAL_1000, ACHIEVEMENT_TOTAL_5000, ACHIEVEMENT_TOTAL_10000, ACHIEVEMENT_TOTAL_20000 };

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

            for (int i = 0; i < individualAchievements.Length; i++)
            {
                if (currentFoodCount >= individualAchievements[i])
                {
                    setAchievement[i](index);
                }
            }

            achievementsManager.SaveAchievementsData();
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

            for (int i = 0; i < individualAchievements.Length; i++)
            {
                if (currentBakersCount >= individualAchievements[i])
                {
                    setAchievement[i](index);
                }
            }

            achievementsManager.SaveAchievementsData();
        }
        #endregion

        // This region's methods check if the player has reached any total cooked food / hired bakers count achievement milestones,
        // considering all food items, based on the total amount.
        // It sets the corresponding achievement property to true if the milestone is reached.
        #region Total Cooked/Hired Achievement
        private void CheckTotalCookedFoodAchievements(object unusedParam = null)
        {
            int totalCookedFoodAmount = TotalCookedFoodAmount();

            Action[] setAchievement = {
                () => Achievements.TotalCooked100 = true,
                () => Achievements.TotalCooked1000 = true,
                () => Achievements.TotalCooked5000 = true,
                () => Achievements.TotalCooked10000 = true,
                () => Achievements.TotalCooked20000 = true
                                                          };

            for (int i = 0; i < totalAchievements.Length; i++)
            {
                if (totalCookedFoodAmount >= totalAchievements[i])
                {
                    setAchievement[i]();
                }
            }

            achievementsManager.SaveAchievementsData();
        }

        private void CheckTotalHiredBakersAchievements(object unusedParam = null)
        {
            int totalHiredBakersAmount = TotalHiredBakersAmount();

            Action[] setAchievement = {
                () => Achievements.TotalHired100 = true,
                () => Achievements.TotalHired1000 = true,
                () => Achievements.TotalHired5000 = true,
                () => Achievements.TotalHired10000 = true,
                () => Achievements.TotalHired20000 = true
                                                         };

            for (int i = 0; i < totalAchievements.Length; i++)
            {
                if (totalHiredBakersAmount >= totalAchievements[i])
                {
                    setAchievement[i]();
                }
            }

            achievementsManager.SaveAchievementsData();
        }
        #endregion

        private int TotalCookedFoodAmount()
        {
            int totalCookedFoodAmount = 0;

            for (int i = 0; i < DBFoodManager.FOOD_COUNT; i++)
            {
                totalCookedFoodAmount += GetFoodData(i).FoodCookedCount;
            }

            return totalCookedFoodAmount;
        }

        private int TotalHiredBakersAmount()
        {
            int totalHiredBakersAmount = 0;

            for (int i = 0; i < DBFoodManager.FOOD_COUNT; i++)
            {
                totalHiredBakersAmount += GetFoodData(i).BakersCount;
            }

            return totalHiredBakersAmount;
        }

        private void RegisterEvents()
        {
            AddListener(DBEventNames.CheckCookedAchievement, CheckCookedFoodAchievements);
            AddListener(DBEventNames.CheckHiredAchievement, CheckHiredBakersAchievements);
            AddListener(DBEventNames.CheckTotalHiredAchievement, CheckTotalHiredBakersAchievements);
            AddListener(DBEventNames.CheckTotalCookedAchievement, CheckTotalCookedFoodAchievements);
        }

        private void UnregisterEvents()
        {
            RemoveListener(DBEventNames.CheckCookedAchievement, CheckCookedFoodAchievements);
            RemoveListener(DBEventNames.CheckHiredAchievement, CheckHiredBakersAchievements);
            RemoveListener(DBEventNames.CheckTotalHiredAchievement, CheckTotalHiredBakersAchievements);
            RemoveListener(DBEventNames.CheckTotalCookedAchievement, CheckTotalCookedFoodAchievements);
        }
    }
}