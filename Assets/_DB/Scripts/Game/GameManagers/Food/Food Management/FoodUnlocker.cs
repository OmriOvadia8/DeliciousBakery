using DB_Core;

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

        public void LearnRecipe(int foodIndex)
        {
            var foodData = foodDataRepository.GetFoodData(foodIndex);

            if (DBGameLogic.Instance.ScoreManager.TryUseScore(ScoreTags.GameCurrency, foodData.UnlockCost))
            {
                foodData.IsFoodLocked = false;
                InvokeUnlockEvents(foodIndex);
                foodDataRepository.SaveFoodData();
            }
            else
            {
                DBDebug.LogException("Not enough money to unlock recipe!");
            }
        }

        private void InvokeUnlockEvents(int foodIndex)
        {
            dbManager.EventsManager.InvokeEvent(DBEventNames.LearnParticles, foodIndex);
            dbManager.EventsManager.InvokeEvent(DBEventNames.FoodBarReveal, foodIndex);
            dbManager.EventsManager.InvokeEvent(DBEventNames.DeviceAppearAnimation, foodIndex);
            dbManager.EventsManager.InvokeEvent(DBEventNames.OnLearnRecipeSpentToast, foodIndex);
            dbManager.EventsManager.InvokeEvent(DBEventNames.PlaySound, SoundEffectType.LearnButtonClick);
            dbManager.EventsManager.InvokeEvent(DBEventNames.BakerPing, true);
        }
    }
}