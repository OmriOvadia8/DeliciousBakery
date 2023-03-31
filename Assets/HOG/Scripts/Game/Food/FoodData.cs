using Core;

namespace Game
{
    public class FoodData
    {
        private bool _isOnCooldown;
        private bool _isAutoOnCooldown;

        public int Index { get; set; }
        public float CookingTime { get; set; }
        public int Profit { get; set; }
        public int UpgradeCost { get; set; }
        public bool IsIdleFood { get; set; }  
        public int HireCost { get; set; }
        public int BakersCount { get; set; }
        public int CookFoodTimes { get; set; }
        public bool IsFoodLocked { get; set; }
        public int UnlockCost { get; set; }
        public bool IsOnCooldown { get; set; }
        public float RemainingCookingTime { get; set; }

        public FoodData()
        {
        }

        public FoodData(int index, float cookingTime, int profit, int upgradeCost, bool isIdleFood, int hireCost, int bakersCount, int cookFoodTimes, bool isFoodUnlocked, int unlockCost, bool isOnCooldown, float remainingCookingTime)
        {
            Index = index;
            CookingTime = cookingTime;
            Profit = profit;
            UpgradeCost = upgradeCost;
            IsIdleFood = isIdleFood;
            HireCost = hireCost;
            BakersCount = bakersCount;
            CookFoodTimes = cookFoodTimes;
            IsFoodLocked = isFoodUnlocked;
            UnlockCost = unlockCost;
            IsOnCooldown = isOnCooldown;
            RemainingCookingTime = remainingCookingTime;
        }

        public bool IsAutoOnCooldown
        {
            get { return _isAutoOnCooldown; }
            set { _isAutoOnCooldown = value; }
        }
    }

    public class FoodDataCollection : IHOGSaveData
    {
        public FoodData[] Foods { get; set; }
    }
}
