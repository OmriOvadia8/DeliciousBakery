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
        
        public static readonly int[] FoodItemAchievementGoals = { 1, 2, 3, 4, 5 }; //{ 1, 3, 500, 1000, 5000 };
        public static readonly int[] GlobalAchievementGoals = { 100, 1000, 5000, 10000, 20000 };

        public static readonly int[] FoodItemsAchievementsRewards = { 100, 200, 300, 500, 1000 };
        public static readonly int[] GlobalAchievementsRewards = { 200, 400, 750, 1250, 2000 };

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

            for (int i = 0; i < GlobalAchievementGoals.Length; i++)
            {
                if (totalCookedFoodAmount >= GlobalAchievementGoals[i])
                {
                    setAchievement[i]();
                    achievementsManager.SaveAchievementsData();
                }
            }  
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

            for (int i = 0; i < GlobalAchievementGoals.Length; i++)
            {
                if (totalHiredBakersAmount >= GlobalAchievementGoals[i])
                {
                    setAchievement[i]();
                    achievementsManager.SaveAchievementsData();
                }
            }    
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