using DB_Core;

namespace DB_Game
{
    public class BakerFoodManager : IBakerFoodManager
    {
        private IFoodDataRepository foodDataRepository;
        private DBManager dbManager;

        private const float BAKER_COST_INCREASE = 1.3f;

        public BakerFoodManager(IFoodDataRepository foodDataRepository, DBManager dbManager)
        {
            this.foodDataRepository = foodDataRepository;
            this.dbManager = dbManager;
        }

        public void UnlockOrUpgradeBakerCooking(int foodIndex)
        {
            var foodData = foodDataRepository.GetFoodData(foodIndex);
            int hireCost = foodData.HireCost;

            if (DBGameLogic.Instance.ScoreManager.TryUseScore(ScoreTags.GameCurrency, hireCost))
            {
                PerformBakerUnlockOrUpgrade(foodData, foodIndex);
            }
            else
            {
                DBDebug.LogException("Failed to unlock/upgrade baker");
            }

            foodDataRepository.SaveFoodData();
        }

        private void PerformBakerUnlockOrUpgrade(FoodData foodData, int foodIndex)
        {
            dbManager.EventsManager.InvokeEvent(DBEventNames.BakerParticles, foodIndex);
            dbManager.EventsManager.InvokeEvent(DBEventNames.OnHireMoneySpentToast, foodIndex);
            foodData.IsBakerUnlocked = true;
            dbManager.EventsManager.InvokeEvent(DBEventNames.StartBakerCooking, foodIndex);
            foodData.HireCost = (int)(foodData.HireCost * BAKER_COST_INCREASE);

            if (foodData.BakersCount % 3 == 0)
            {
                foodData.CookFoodMultiplier++;
            }

            foodData.BakersCount++;
            dbManager.EventsManager.InvokeEvent(DBEventNames.OnHiredTextUpdate, foodIndex);
        }
    }
}