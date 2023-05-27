using DB_Core;
using System;

namespace DB_Game
{
    public class BakerFoodManager : IBakerFoodManager
    {
        private IFoodDataRepository foodDataRepository;
        private DBManager dbManager;

        private const float BAKER_COST_BASE_INCREASE = 1.2f;
        private const float BAKER_COST_GROWTH_FACTOR = 1.3f; // can tweak this to adjust the rate of growth
        private const int HIRE_COST_MILESTONE = 10;
        private const int SPECIAL_BAKER = 5;
        private const float BAKER_COST_INCREASE_SPECIAL = 2f; // multiplier for special bakers


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

            double hireCostIncrease = BAKER_COST_BASE_INCREASE * Math.Pow(BAKER_COST_GROWTH_FACTOR, foodData.BakersCount / HIRE_COST_MILESTONE);

            if ((foodData.BakersCount + 2) % SPECIAL_BAKER == 0)
            {
                hireCostIncrease *= BAKER_COST_INCREASE_SPECIAL;
            }

            foodData.HireCost *= hireCostIncrease;
            foodData.BakersCount++;
            foodData.CookFoodMultiplier = CalculateCookFoodMultiplier(foodData.BakersCount);

            InvokeHireEvents(foodIndex);
        }
        
        private int CalculateCookFoodMultiplier(int bakerCount)
        {
            if (bakerCount < 1) return 0;
            if (bakerCount < 2) return 1;
            if (bakerCount < 5) return 2;
            if (bakerCount < 10) return 3;
            return 2 + (bakerCount / 5);
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