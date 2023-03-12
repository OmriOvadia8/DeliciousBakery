using Core;
// TODO: Hire Cost to json, not interactable upgrades when not enough $, working baker upgrade system (increase profit x Times), sleep on it...
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
        public int HireCost { get; set; }

        public FoodData()
        {
        }

        public FoodData(int index, float cookingTime, int profit, int upgradeCost, bool isIdleFood, int hireCost)
        {
            Index = index;
            CookingTime = cookingTime;
            Profit = profit;
            UpgradeCost = upgradeCost;
            IsIdleFood = isIdleFood;
            HireCost = hireCost;
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
