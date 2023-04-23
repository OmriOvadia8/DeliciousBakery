using DB_Core;

namespace DB_Game
{
    public class PremStoreActions : DBLogicMonoBehaviour
    {
        public void WatchAd()
        {
            DBManager.Instance.AdsManager.ShowAd(null);
        }
    }
}