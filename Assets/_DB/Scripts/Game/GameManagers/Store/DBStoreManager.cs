using System.Collections.Generic;
using System.Linq;
using DB_Core;

namespace DB_Game
{
    public class DBStoreManager
    {
        public DBStoresConfigData StoresConfigData;

        public DBStoreManager() => LoadStoresConfig();

        private void LoadStoresConfig() => 
            DBManager.Instance.ConfigManager.GetConfigAsync("store_config", (DBStoresConfigData config) => StoresConfigData = config);

        public bool TryBuyProduct(string sku, string storeID)
        {
            DBManager.Instance.PurchaseManager.Purchase(sku, isSuccess =>
            {
                if (isSuccess)
                {
                    var product = GetStoreByStoreID(storeID).StoreProducts.First(x => x.SKU == sku);
                    RedeemBundle(product.StoreBundle);
                }
            });

            return false;
        }

        private void RedeemBundle(DBBundle[] productStoreBundle)
        {
            foreach (var bundle in productStoreBundle)
            {
                DBGameLogic.Instance.ScoreManager.ChangeScoreByTagByAmount(bundle.ScoreTag, bundle.ScoreAmount);
            }
        }

        public DBStoreData GetStoreByStoreID(string storeID)
        {
            return StoresConfigData.StoreDatas.FirstOrDefault(x => x.StoreID == storeID);
        }
    }

    public class DBStoresConfigData
    {
        public List<DBStoreData> StoreDatas = new();
    }

    public class DBStoreData
    {
        public string StoreID;
        public string Title;
        public List<DBStoreProduct> StoreProducts = new();
    }

    public class DBStoreProduct
    {
        public string SKU;
        public string ArtName;
        public string SellingText;
        public DBBundle[] StoreBundle;
    }

    public class DBBundle
    {
        public ScoreTags ScoreTag;
        public int ScoreAmount;
    }
}