using Core;
using System.Collections;
using UnityEngine;

namespace Game
{
    public class FoodData
    {
        public string name;
        public int level;
        public float cookingTime;
        public int profit;
        public int levelUpCost;
        

        public FoodData(string name, int level, float cookingTime, int profit, int levelUpCost)
        {
            this.name = name;
            this.level = level;
            this.cookingTime = cookingTime;
            this.profit = profit;
            this.levelUpCost = levelUpCost;
        }
    }
}