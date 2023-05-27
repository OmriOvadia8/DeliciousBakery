using DB_Core;

namespace DB_Game
{
    public class FoodData
    {
        public int Index { get; set; }
        public float CookingTime { get; set; }
        public double Profit { get; set; }
        public double UpgradeCost { get; set; }
        public bool IsBakerUnlocked { get; set; }  
        public double HireCost { get; set; }
        public int BakersCount { get; set; }
        public int CookFoodMultiplier { get; set; }
        public bool IsFoodLocked { get; set; }
        public double UnlockCost { get; set; }
        public bool IsOnCooldown { get; set; }
        public float RemainingCookingTime { get; set; }
        public float BakerCookingTime { get; set; }
        public float RemainingBakerCookingTime { get; set; }
        public bool IsAutoOnCooldown { get; set; }
        public int FoodCookedCount { get; set; }

        public FoodData()
        {
        }

        public FoodData(int index,
                        float cookingTime,
                        double profit,
                        double upgradeCost,
                        bool isBakerUnlocked,
                        double hireCost,
                        int bakersCount,
                        int cookFoodMultiplier,
                        bool isFoodlocked,
                        double unlockCost,
                        bool isOnCooldown,
                        float remainingCookingTime,
                        float bakerCookingTime,
                        float remainingBakerCookingTime,
                        bool isAutoOnCooldown,
                        int foodCookedCount)

        {
            Index = index;
            CookingTime = cookingTime;
            Profit = profit;
            UpgradeCost = upgradeCost;
            IsBakerUnlocked = isBakerUnlocked;
            HireCost = hireCost;
            BakersCount = bakersCount;
            CookFoodMultiplier = cookFoodMultiplier;
            IsFoodLocked = isFoodlocked;
            UnlockCost = unlockCost;
            IsOnCooldown = isOnCooldown;
            RemainingCookingTime = remainingCookingTime;
            BakerCookingTime = bakerCookingTime;
            RemainingBakerCookingTime = remainingBakerCookingTime;
            IsAutoOnCooldown = isAutoOnCooldown;
            FoodCookedCount = foodCookedCount;
        }
    }

    public class FoodDataCollection : IDBSaveData
    {
        public FoodData[] Foods { get; set; }
    }
}
