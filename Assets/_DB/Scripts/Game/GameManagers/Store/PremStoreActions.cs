using DB_Core;

namespace DB_Game
{
    public class PremStoreActions : DBLogicMonoBehaviour
    {
        public void IAPStarsPurchase(int productIndex)
        {
            var storeData = GameLogic.StoreManager.GetStoreByStoreID("1");
            GameLogic.StoreManager.TryBuyProduct(storeData.StoreProducts[productIndex].SKU, storeData.StoreID, isSuccess =>
            {
                if (isSuccess)
                {
                    InvokeEvent(DBEventNames.PremCurrencyUpdateUI, null);
                    InvokeEvent(DBEventNames.CheckBuySkinButtonUI, null);
                    InvokeEvent(DBEventNames.CheckBuyTimeWrapButtonsUI, null);
                    Manager.AnalyticsManager.ReportEvent(DBEventType.purchase_complete);
                    DBDebug.Log("IAP SUCCESS");
                }
                else
                {
                    DBDebug.LogException("IAP FAILED");
                    Manager.AnalyticsManager.ReportEvent(DBEventType.purchase_failed);
                }
            });
        }
    }
}
