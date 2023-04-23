using DB_Core;
using UnityEditor;
using UnityEngine;

namespace DB_Game
{
    public class FoodUnlocker : IFoodUnlocker
    {
        private IFoodDataRepository foodDataRepository;
        private DBManager dbManager;

        public FoodUnlocker(IFoodDataRepository foodDataRepository, DBManager dbManager)
        {
            this.foodDataRepository = foodDataRepository;
            this.dbManager = dbManager;
        }

        public void LearnRecipe(int foodID)
        {
            var foodData = foodDataRepository.GetFoodData(foodID);

            if (DBGameLogic.Instance.ScoreManager.TryUseScore(ScoreTags.GameCurrency, foodData.UnlockCost))
            {
                dbManager.EventsManager.InvokeEvent(DBEventNames.LearnParticles, foodID);
                foodData.IsFoodLocked = false;
                dbManager.EventsManager.InvokeEvent(DBEventNames.FoodBarReveal, foodID);

                dbManager.EventsManager.InvokeEvent(DBEventNames.DeviceAppearAnimation, foodID);
                dbManager.EventsManager.InvokeEvent(DBEventNames.OnLearnRecipeSpentToast, foodID);

                foodDataRepository.SaveFoodData();
            }
            else
            {
                DBDebug.LogException("Not enough money to unlock recipe!");
            }
        }
    }

}