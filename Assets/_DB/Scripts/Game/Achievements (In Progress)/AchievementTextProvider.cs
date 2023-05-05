using DB_Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DB_Game
{
    public class AchievementTextProvider
    {
        private Dictionary<FoodTypes, string[]> foodTitles = new();
        private Dictionary<FoodTypes, string[]> hireBakerTitles = new();

        public AchievementTextProvider(int foodCount)
        {
            for (int i = 0; i < foodCount; i++)
            {
                FoodTypes foodName = (FoodTypes)i;
                foodTitles[foodName] = new string[]
                {
                    $"{foodName} Amateur",
                    $"{foodName} Enthusiast",
                    $"{foodName} Expert",
                    $"{foodName} Master",
                    $"{foodName} Legend"
                };

                hireBakerTitles[foodName] = new string[]
                {
                    $"{foodName} Newbie",
                    $"{foodName} Novice",
                    $"{foodName} Skilled",
                    $"{foodName} Adept",
                    $"{foodName} Elite"
                };
            }
        }

        public string GetMakeFoodTitle(FoodTypes foodName, int milestoneIndex)
        {
            return foodTitles[foodName][milestoneIndex];
        }

        public string GetHireBakerTitle(FoodTypes foodName, int milestoneIndex)
        {
            return hireBakerTitles[foodName][milestoneIndex];
        }

        public string GetMakeFoodDescription(FoodTypes foodName, int amount)
        {
            return $"Make {amount} {foodName}";
        }

        public string GetHireBakerDescription(FoodTypes foodName, int amount)
        {
            return $"Hire {amount} {foodName} Bakers";
        }
    }
}
