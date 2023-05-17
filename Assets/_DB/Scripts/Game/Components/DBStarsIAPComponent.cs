using DB_Core;

namespace DB_Game
{
    public class DBStarsIAPComponent : DBLogicMonoBehaviour
    {
        private const int FIRST_ITEM_STARS_GAIN = 1000;
        private const int SECOND_ITEM_STARS_GAIN = 1800;
        private const int THIRD_ITEM_STARS_GAIN = 3300;

        public void FirstIAPItem() => AddStarsOnPurchase(FIRST_ITEM_STARS_GAIN);

        public void SecondIAPItem() => AddStarsOnPurchase(SECOND_ITEM_STARS_GAIN);

        public void ThirdIAPItem() => AddStarsOnPurchase(THIRD_ITEM_STARS_GAIN);

        private void AddStarsOnPurchase(int stars)
        {
            InvokeEvent(DBEventNames.AddStarsUpdate, stars);
            InvokeEvent(DBEventNames.PremCurrencyUpdateUI, null);
            InvokeEvent(DBEventNames.CheckBuySkinButtonUI, null);
            InvokeEvent(DBEventNames.CheckBuyTimeWrapButtonsUI, null);
        }
    }
}