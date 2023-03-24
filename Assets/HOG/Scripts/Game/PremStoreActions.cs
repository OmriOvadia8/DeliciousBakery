using Core;
using UnityEngine;

namespace Game
{
    public class PremStoreActions : HOGLogicMonoBehaviour
    {
        public void WatchAd()
        {
            HOGManager.Instance.AdsManager.ShowAd(null);
        }
    }
}