using DB_Core;

namespace DB_Game
{
    public class FoodUpgrader : IFoodUpgrader
    {
        private IFoodDataRepository foodDataRepository;
        private DBManager dbManager;

        private const float PROFIT_INCREASE = 1.1f;
        private const float COST_INCREASE = 1.15f;

        public FoodUpgrader(IFoodDataRepository foodDataRepository, DBManager dbManager)
        {
            this.foodDataRepository = foodDataRepository;
            this.dbManager = dbManager;
        }

        public void UpgradeFood(int foodID)
        {
            if (!foodDataRepository.IsFoodDataLoaded())
            {
                DBDebug.LogException("Food data not loaded");
                return;
            }

            var foodData = foodDataRepository.GetFoodData(foodID);

            if (foodData == null)
            {
                DBDebug.LogException("Invalid food ID: " + foodID);
                return;
            }

            int initialLevel = DBGameLogic.Instance.UpgradeManager.GetUpgradeableByID(UpgradeablesTypeID.Food, foodID).CurrentLevel;
            int initialUpgradeCost = foodData.UpgradeCost;

            DBGameLogic.Instance.UpgradeManager.UpgradeItemByID(UpgradeablesTypeID.Food, foodID, ScoreTags.GameCurrency, initialUpgradeCost);

            if (initialLevel < DBGameLogic.Instance.UpgradeManager.GetUpgradeableByID(UpgradeablesTypeID.Food, foodID).CurrentLevel)
            {
                dbManager.EventsManager.InvokeEvent(DBEventNames.OnUpgradeMoneySpentToast, foodID);
                dbManager.EventsManager.InvokeEvent(DBEventNames.UpgradeParticles, foodID);
                foodData.Profit = (int)(foodData.Profit * PROFIT_INCREASE);
                foodData.UpgradeCost = (int)(foodData.UpgradeCost * COST_INCREASE);

                dbManager.EventsManager.InvokeEvent(DBEventNames.OnUpgraded, foodID);

                foodDataRepository.SaveFoodData();
            }

            DBDebug.Log(DBGameLogic.Instance.UpgradeManager.GetUpgradeableByID(UpgradeablesTypeID.Food, foodID).CurrentLevel);
        }
    }

}