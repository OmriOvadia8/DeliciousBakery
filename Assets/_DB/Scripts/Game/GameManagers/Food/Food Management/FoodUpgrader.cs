using DB_Core;
using System;

namespace DB_Game
{
    public class FoodUpgrader : IFoodUpgrader
    {
        private IFoodDataRepository foodDataRepository;
        private DBManager dbManager;
        private const double UPGRADE_COST_GROWTH = 1.04; 
        private const double PROFIT_GROWTH = 1.02; 

        public FoodUpgrader(IFoodDataRepository foodDataRepository, DBManager dbManager)
        {
            this.foodDataRepository = foodDataRepository;
            this.dbManager = dbManager;
        }

        public void UpgradeFood(int foodIndex)
        {
            if (!IsValidFoodData(foodIndex))
            {
                return;
            }

            var foodData = foodDataRepository.GetFoodData(foodIndex);
            int initialLevel = GetCurrentFoodLevel(foodIndex);
            double initialUpgradeCost = foodData.UpgradeCost;

            UpgradeFoodItem(foodIndex, initialUpgradeCost);

            if (DidFoodLevelIncrease(initialLevel, foodIndex))
            {
                PerformPostUpgradeActions(foodData, foodIndex);
            }
        }

        private int GetCurrentFoodLevel(int foodIndex) => 
            DBGameLogic.Instance.UpgradeManager.GetUpgradeableByID(UpgradeablesTypeID.Food, foodIndex).CurrentLevel;

        private void UpgradeFoodItem(int foodIndex, double initialUpgradeCost) =>
            DBGameLogic.Instance.UpgradeManager.UpgradeItemByID(UpgradeablesTypeID.Food, foodIndex, ScoreTags.GameCurrency, initialUpgradeCost);

        private bool DidFoodLevelIncrease(int initialLevel, int foodIndex) =>
            initialLevel < DBGameLogic.Instance.UpgradeManager.GetUpgradeableByID(UpgradeablesTypeID.Food, foodIndex).CurrentLevel;


        private void PerformPostUpgradeActions(FoodData foodData, int foodIndex)
        {
            dbManager.EventsManager.InvokeEvent(DBEventNames.OnUpgradeMoneySpentToast, foodIndex);
            int level = DBGameLogic.Instance.UpgradeManager.GetUpgradeableByID(UpgradeablesTypeID.Food, foodIndex).CurrentLevel;

            foodData.Profit *= Math.Pow(PROFIT_GROWTH, level);
            foodData.UpgradeCost *= Math.Pow(UPGRADE_COST_GROWTH, level);

            InvokeFoodUpgradeEvents(foodIndex);
            foodDataRepository.SaveFoodData();
        }


        private void InvokeFoodUpgradeEvents(int foodIndex)
        {
            dbManager.EventsManager.InvokeEvent(DBEventNames.UpgradeParticles, foodIndex);
            dbManager.EventsManager.InvokeEvent(DBEventNames.OnUpgradeTextUpdate, foodIndex);
            dbManager.EventsManager.InvokeEvent(DBEventNames.TimeWrapCoinsText, null);
            dbManager.EventsManager.InvokeEvent(DBEventNames.PlaySound, SoundEffectType.UpgradeButtonClick);
        }

        private bool IsValidFoodData(int foodIndex)
        {
            if (!foodDataRepository.IsFoodDataLoaded())
            {
                DBDebug.LogException("Food data not loaded");
                return false;
            }

            var foodData = foodDataRepository.GetFoodData(foodIndex);

            if (foodData == null)
            {
                DBDebug.LogException("Invalid food ID: " + foodIndex);
                return false;
            }

            return true;
        }
    }
}
