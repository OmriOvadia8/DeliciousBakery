using DB_Core;
using System;

namespace DB_Game
{
    public class BakerFoodManager : IBakerFoodManager
    {
        private IFoodDataRepository foodDataRepository;
        private DBManager dbManager;
        private const double BAKER_COST_GROWTH = 1.04;
        private const double BAKER_MULTIPLE_INCREASE = 0.25;

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
                DBDebug.LogException("Failed / no money to unlock/upgrade baker");
            }

            foodDataRepository.SaveFoodData();
        }

        private void PerformBakerUnlockOrUpgrade(FoodData foodData, int foodIndex)
        {
            foodData.IsBakerUnlocked = true;
            dbManager.EventsManager.InvokeEvent(DBEventNames.OnHireMoneySpentToast, foodIndex);

            // Calculate the new hire cost
            foodData.HireCost *= Math.Pow(BAKER_COST_GROWTH, foodData.BakersCount + 1);

            foodData.BakersCount++;

            if (foodData.BakersCount == 1)
            {
                foodData.CookFoodMultiplier = 1;
            }
            else
            {
                // Increase the efficiency of each baker by 5% over the previous level
                foodData.CookFoodMultiplier += BAKER_MULTIPLE_INCREASE;
            }

            InvokeHireEvents(foodIndex);
        }

        private void InvokeHireEvents(int foodIndex)
        {
            dbManager.EventsManager.InvokeEvent(DBEventNames.BakerParticles, foodIndex);
            dbManager.EventsManager.InvokeEvent(DBEventNames.StartBakerCooking, foodIndex);
            dbManager.EventsManager.InvokeEvent(DBEventNames.OnHiredTextUpdate, foodIndex);
            dbManager.EventsManager.InvokeEvent(DBEventNames.CheckHiredAchievement, foodIndex);
            dbManager.EventsManager.InvokeEvent(DBEventNames.CurrentHireBakerAchievementStatus, foodIndex);
            dbManager.EventsManager.InvokeEvent(DBEventNames.PlaySound, SoundEffectType.HireSound);
            dbManager.EventsManager.InvokeEvent(DBEventNames.SlimeAction, foodIndex);
            dbManager.EventsManager.InvokeEvent(DBEventNames.TimeWrapCoinsText, null);
            dbManager.AnalyticsManager.ReportEvent(DBEventType.hire_baker);
        }
    }
}