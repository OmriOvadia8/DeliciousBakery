using System;
using UnityEngine.Advertisements;

namespace DB_Core
{
    public class DBAdsManager : IUnityAdsLoadListener, IUnityAdsShowListener
    {
        private bool isAdLoaded;
        private Action<bool> onShowComplete;
        private Action<bool> onShowRewardedComplete;

        public DBAdsManager()
        {
            Advertisement.Initialize("5215648");
            LoadAd();
        }

        private void LoadAd()
        {
            Advertisement.Load("Interstitial_Android", this);
        }

        public void ShowAd(Action<bool> onShowAdComplete)
        {
            if (isAdLoaded)
            {
                onShowAdComplete.Invoke(false);
                return;
            }

            onShowComplete = onShowAdComplete;
            Advertisement.Show("Interstitial_Android", this);
        }

        public void ShowAdReward(Action<bool> onShowAdComplete)
        {
            if (isAdLoaded)
            {
                onShowAdComplete.Invoke(false);
                return;
            }

            onShowRewardedComplete = onShowAdComplete;
            Advertisement.Show("Rewarded_Android", this);
        }

        public void OnUnityAdsAdLoaded(string placementId)
        {
            isAdLoaded = true;
            DBDebug.Log("OnUnityAdsAdLoaded");
        }

        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
        {
            isAdLoaded = false;
            DBManager.Instance.CrashManager.LogExceptionHandling(message);
        }

        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
        {
            DBManager.Instance.CrashManager.LogExceptionHandling(message);

            onShowComplete?.Invoke(false);
            onShowRewardedComplete?.Invoke(false);

            onShowComplete = null;
            onShowRewardedComplete = null;
        }

        public void OnUnityAdsShowStart(string placementId)
        {
            DBManager.Instance.AnalyticsManager.ReportEvent(DBEventType.ad_show_start);
        }

        public void OnUnityAdsShowClick(string placementId)
        {
            DBManager.Instance.AnalyticsManager.ReportEvent(DBEventType.ad_show_click);
            onShowComplete?.Invoke(true);
        }

        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            DBManager.Instance.AnalyticsManager.ReportEvent(DBEventType.ad_show_complete);
            onShowComplete?.Invoke(true);

            if (showCompletionState == UnityAdsShowCompletionState.COMPLETED && placementId == "Rewarded_Android")
            {
                onShowRewardedComplete?.Invoke(true);
            }

            onShowComplete = null;
            onShowRewardedComplete = null;
        }
    }
}