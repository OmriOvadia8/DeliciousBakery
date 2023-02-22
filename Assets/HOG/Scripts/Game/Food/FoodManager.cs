using Core;
using UnityEngine;

namespace Game
{
    public class FoodManager : HOGLogicMonoBehaviour
    {
        private FoodData[] foods;
        public const int FOOD_COUNT = 10; // total food types count in game
        private const float PROFIT_INCREASE = 1.1f; // increasing the food's profit by 10% each upgrade
        private const float COST_INCREASE = 1.15f; // increasing the upgrade's cost by 15% each upgrade
 
        void Awake() // on awake creating an array with all foods with their stats: Cooking Time, Profit, Upgrade Cost.
        {
            foods = new FoodData[FOOD_COUNT];

            foods[(int)FoodType.Cookie] = new FoodData(2, 18, 60);
            foods[(int)FoodType.Chocolates] = new FoodData(2, 25, 100);
            foods[(int)FoodType.Donut] = new FoodData(3, 36, 200);
            foods[(int)FoodType.Icecream] = new FoodData(4, 45, 240);
            foods[(int)FoodType.Cupcake] = new FoodData(4, 60, 350);
            foods[(int)FoodType.Brownie] = new FoodData(5, 80, 500);
            foods[(int)FoodType.Cheesecake] = new FoodData(7, 110, 850);
            foods[(int)FoodType.Pizza] = new FoodData(10, 160, 1100);
            foods[(int)FoodType.Cake] = new FoodData(10, 200, 1500);
            foods[(int)FoodType.Teabox] = new FoodData(15, 350, 2000);
        }

        private void Start() // On start loop to add all the foods to the list and update text to each food's stats
        {
            for (int i = 0; i < FOOD_COUNT; i++)
            {
                AddNewFoodItem(i);
                InvokeEvent(HOGEventNames.OnUpgraded, i);
            }
        }

        private void AddNewFoodItem(int foodID) // adding each food from the array to the upgradeables list
        {
            GameLogic.UpgradeManager.PlayerUpgradeInventoryData.Upgradeables.Add(new HOGUpgradeableData
            {
                upgradableTypeID = UpgradeablesTypeID.Food,
                CurrentLevel = 1,
                foodID = foodID
            });
        }

        public void UpgradeFood(int foodID) // upgrading food results in increasing the level by 1 which then affects the other stats
        {
            var foodData = foods[foodID];
            var upgradeableType = UpgradeablesTypeID.Food;

            GameLogic.UpgradeManager.UpgradeItemByID(upgradeableType, foodID);

            foodData.Profit = (int)(foodData.Profit * PROFIT_INCREASE); // 10% increase income
            foodData.LevelUpCost = (int)(foodData.LevelUpCost * COST_INCREASE); // 15% increase upgrade cost

            InvokeEvent(HOGEventNames.OnUpgraded, foodID); // updates UI

            Debug.Log(GameLogic.UpgradeManager.GetUpgradeableByID(upgradeableType, foodID).CurrentLevel);
        }

        public FoodData GetFoodData(int foodIndex) // gather the food's stats info
        {
            return foods[foodIndex];
        }

        public bool IsFoodOnCooldown(int foodIndex) // checks if food is being cooked
        {
            return foods[foodIndex].IsOnCooldown;
        }

        public void SetFoodOnCooldown(int foodIndex, bool value) // set food to being cooked or not
        {
            foods[foodIndex].IsOnCooldown = value;
        }
    }

    public enum FoodType
    {
        Cookie = 0,
        Chocolates = 1,
        Donut = 2,
        Icecream = 3,
        Cupcake = 4,
        Brownie = 5,
        Cheesecake = 6,
        Pizza = 7,
        Cake = 8,
        Teabox = 9
    }
}