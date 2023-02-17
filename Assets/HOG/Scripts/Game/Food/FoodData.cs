using Core;
using System.Collections;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using System;

namespace Game
{
    public class FoodData
    {
        private readonly string name;
        private int level;
        private float cookingTime;
        private int profit;
        private int levelUpCost;
        private bool isOnCooldown;
        private const int STARTING_LEVEL = 1;

        public FoodData(string name, float cookingTime, int profit, int levelUpCost)
        {
            this.name = name;
            level = STARTING_LEVEL;
            this.cookingTime = cookingTime;
            this.profit = profit;
            this.levelUpCost = levelUpCost;
            isOnCooldown = false;
        }

        public int Level
        {
            get { return level; }
            set { level = value; }
        }

        public bool IsOnCooldown
        {
            get { return isOnCooldown; }
            set
            {
                if (value)
                {
                    isOnCooldown = true;
                }
                else
                {
                    isOnCooldown = false;
                }
            }
        }

        public string Name
        {
            get { return name; }
        }

        public float CookingTime
        {
            get { return cookingTime; }
            set { cookingTime = value; }
        }

        public int Profit
        {
            get { return profit; }
            set { profit = value; }
        }

        public int LevelUpCost
        {
            get { return levelUpCost; }
            set { levelUpCost = value; }
        }
    }
}