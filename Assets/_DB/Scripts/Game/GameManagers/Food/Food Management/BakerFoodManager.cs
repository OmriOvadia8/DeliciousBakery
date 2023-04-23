using DB_Core;
using UnityEditor;
using UnityEngine;

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

        public void UnlockOrUpgradeIdleFood(int foodIndex)
        {
            var foodData = foodDataRepository.GetFoodData(foodIndex);

            if (DBGameLogic.Instance.ScoreManager.TryUseScore(ScoreTags.GameCurrency, foodData.HireCost))
            {
                dbManager.EventsManager.InvokeEvent(DBEventNames.BakerParticles, foodIndex);
                dbManager.EventsManager.InvokeEvent(DBEventNames.OnHireMoneySpentToast, foodIndex);
                foodData.IsIdleFood = true;
                dbManager.EventsManager.InvokeEvent(DBEventNames.StartBakerCooking, foodIndex);
                foodData.HireCost = (int)(foodData.HireCost * BAKER_COST_INCREASE);
                if (foodData.BakersCount % 3 == 0)
                {
                    foodData.CookFoodTimes++;
                }
                foodData.BakersCount++;
                DBManager.Instance.EventsManager.InvokeEvent(DBEventNames.OnHired, foodIndex);
            }
            else
            {
                DBDebug.LogException("Failed to unlock/update idle");
            }

            foodDataRepository.SaveFoodData();
        }
    }
}