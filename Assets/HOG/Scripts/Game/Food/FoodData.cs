using Core;

namespace Game
{
    public class FoodData
    {
        private bool _isOnCooldown;
        public int Index { get; set; }
        public float CookingTime { get; set; }
        public int Profit { get; set; }
        public int UpgradeCost { get; set; }
        public bool IsIdleFood { get; set; }  

        public FoodData()
        {
        }

        public FoodData(int index, float cookingTime, int profit, int upgradeCost, bool isIdleFood)
        {
            Index = index;
            CookingTime = cookingTime;
            Profit = profit;
            UpgradeCost = upgradeCost;
            IsIdleFood = isIdleFood;
        }

        public bool IsOnCooldown
        {
            get { return _isOnCooldown; }
            set { _isOnCooldown = value; }
        }
    }

    public class FoodDataCollection : IHOGSaveData
    {
        public FoodData[] Foods { get; set; }
    }
}
