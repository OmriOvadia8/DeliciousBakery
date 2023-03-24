using System.Collections.Generic;
using System.Linq;
using Core;

namespace Game
{
    public class HOGStoreManager
    {
        public HOGStoresConfigData StoresConfigData;

        public bool TryBuyProduct(string sku, string storeID)
        {
            HOGManager.Instance.PurchaseManager.Purchase(sku, isSuccess =>
            {
                if (isSuccess)
                {
                    var product = GetStoreByStoreID(storeID).StoreProducts.First(x => x.SKU == sku);
                    RedeemBundle(product.StoreBundle);
                }
            });

            return false;
        }

        private void RedeemBundle(HOGBundle[] productStoreBundle)
        {
            foreach (var bundle in productStoreBundle)
            {
                HOGGameLogic.Instance.ScoreManager.ChangeScoreByTagByAmount(bundle.ScoreTag, bundle.ScoreAmount);
            }
        }

        public HOGStoreData GetStoreByStoreID(string storeID)
        {
            return StoresConfigData.StoreDatas.FirstOrDefault(x => x.StoreID == storeID);
        }

    }

    public class HOGStoresConfigData
    {
        public List<HOGStoreData> StoreDatas = new();
    }

    public class HOGStoreData
    {
        public string StoreID;
        public string Title;
        public List<HOGStoreProduct> StoreProducts = new();
    }

    public class HOGStoreProduct
    {
        public string SKU;
        public string ArtName;
        public string SellingText;
        public HOGBundle[] StoreBundle;
    }

    public class HOGBundle
    {
        public ScoreTags ScoreTag;
        public int ScoreAmount;
    }
}