using DB_Core;

namespace DB_Game
{
    public class BakerFoodManager : IBakerFoodManager
    {
        private IFoodDataRepository foodDataRepository;
        private DBManager dbManager;

        private const float BAKER_COST_INCREASE = 1.3f;
        private const int PER_BAKERS = 3;

        public BakerFoodManager(IFoodDataRepository foodDataRepository, DBManager dbManager)
        {
            this.foodDataRepository = foodDataRepository;
            this.dbManager = dbManager;
        }

        public void UnlockOrUpgradeBakerCooking(int foodIndex)
        {
            var foodData = foodDataRepository.GetFoodData(foodIndex);
            double hireCost = foodData.HireCost;

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
            foodData.IsBakerUnlocked = true;
            foodData.HireCost = (double)(foodData.HireCost * BAKER_COST_INCREASE);

            if (foodData.BakersCount % PER_BAKERS == 0)
            {
                foodData.CookFoodMultiplier++;
            }

            foodData.BakersCount++;
            InvokeHireEvents(foodIndex);
        }

        private void InvokeHireEvents(int foodIndex)
        {
            dbManager.EventsManager.InvokeEvent(DBEventNames.BakerParticles, foodIndex);
            dbManager.EventsManager.InvokeEvent(DBEventNames.OnHireMoneySpentToast, foodIndex);
            dbManager.EventsManager.InvokeEvent(DBEventNames.StartBakerCooking, foodIndex);
            dbManager.EventsManager.InvokeEvent(DBEventNames.OnHiredTextUpdate, foodIndex);
            dbManager.EventsManager.InvokeEvent(DBEventNames.CheckHiredAchievement, foodIndex);
            dbManager.EventsManager.InvokeEvent(DBEventNames.CurrentHireBakerAchievementStatus, foodIndex);
            dbManager.EventsManager.InvokeEvent(DBEventNames.PlaySound, SoundEffectType.HireSound);
            dbManager.EventsManager.InvokeEvent(DBEventNames.SlimeAction, foodIndex);
            dbManager.EventsManager.InvokeEvent(DBEventNames.TimeWrapCoinsText, null);
        }
    }
}