using System;
using UnityEngine.Advertisements;

namespace DB_Core
{
    public class DBAdsManager : IUnityAdsLoadListener, IUnityAdsShowListener, IUnityAdsInitializationListener
    {
        private bool isAdLoaded;
        private Action<bool> onShowComplete;
        private Action<bool> onShowRewardedComplete;
        private readonly string gameID = "5215648";
        private readonly string adUnit = "Interstitial_Android";

        public DBAdsManager()
        {
            Advertisement.Initialize(gameID, false, this);
            LoadAd();
        }

        private void LoadAd()
        {
            DBDebug.Log("Loading Ad");
            Advertisement.Load(adUnit, this);
        }

        public void ShowAd()
        {
            Advertisement.Show(adUnit, this);
        }

        public void ShowAdReward(Action<bool> onShowAdComplete)
        {
            if (!isAdLoaded)
            {
                onShowAdComplete?.Invoke(false);
                return;
            }

            onShowRewardedComplete = onShowAdComplete;
            Advertisement.Show("Rewarded_Android", this);
        }

        public void OnUnityAdsAdLoaded(string placementId)
        {
            if (placementId == adUnit)
            {
                isAdLoaded = true;
                DBDebug.Log("Ad loaded");
            }
        }

        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
        {
            isAdLoaded = false;
            DBManager.Instance.CrashManager.LogExceptionHandling(message);
            DBDebug.Log($"OnUnityAdsFailedToLoad: {placementId}, Error: {error}, Message: {message}");
        }

        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
        {
            DBManager.Instance.CrashManager.LogExceptionHandling(message);

            onShowComplete?.Invoke(false);
            onShowRewardedComplete?.Invoke(false);

            onShowComplete = null;
            onShowRewardedComplete = null;
            DBDebug.Log("OnUnityAdsShowFailure");
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

            LoadAd(); 
        }

        public void OnInitializationComplete()
        {
            DBDebug.Log("Unity Ads initialization complete.");
            LoadAd();
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            DBDebug.LogException($"Unity Ads initialization failed: {error} - {message}");
        }
    }
}