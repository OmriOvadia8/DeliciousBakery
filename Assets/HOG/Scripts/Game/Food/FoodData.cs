namespace Game
{
    public class FoodData
    {
        private bool _isOnCooldown;

        public string Name { get; }
        public float CookingTime { get; set; }
        public int Profit { get; set; }
        public int LevelUpCost { get; set; }

        public FoodData(float cookingTime, int profit, int levelUpCost)
        {
            CookingTime = cookingTime;
            Profit = profit;
            LevelUpCost = levelUpCost;

            _isOnCooldown = false;
        }

        public bool IsOnCooldown
        {
            get { return _isOnCooldown; }
            set { _isOnCooldown = value; }
        }
    }
}
