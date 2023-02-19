namespace Game
{
    public class FoodData
    {
        private bool _isOnCooldown;

        public float CookingTime { get; set; }
        public int Profit { get; set; }
        public int LevelUpCost { get; set; }

        public FoodData(float cookingTime, int profit, int levelUpCost)
        {
            _isOnCooldown = false;

            CookingTime = cookingTime;
            Profit = profit;
            LevelUpCost = levelUpCost;         
        }

        public bool IsOnCooldown
        {
            get { return _isOnCooldown; }
            set { _isOnCooldown = value; }
        }
    }
}
