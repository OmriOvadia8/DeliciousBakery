using DB_Core;
using System;
using UnityEngine;
using TMPro;
using DG.Tweening;

namespace DB_Game
{
    public class DBDoubleProfitController : DBLogicMonoBehaviour
    {
        private bool isDoubleProfitOn;
        public static int DoubleProfitMultiplier = 1;
        private DoubleProfitData doubleProfitSaveData;
        private const string PROFIT_TWEEN = "Profit";
        [SerializeField] private TMP_Text doubleProfitTimerText;
        [SerializeField] private GameObject timer;

        private void Awake() => LoadDoubleProfit();

        private void OnEnable() => AddListener(DBEventNames.OnPause, DoubleProfitPauseCheck);

        private void OnDisable() => RemoveListener(DBEventNames.OnPause, DoubleProfitPauseCheck);

        private void Start()
        {
            timer.SetActive(false);

            bool isDoubleProfitActivated = doubleProfitSaveData.IsDoubleProfitOn;
            int offlineTime = Manager.TimerManager.GetLastOfflineTimeSeconds();

            if (isDoubleProfitActivated && offlineTime > 0)
            {
                ContinueDoubleProfitAfterPause();
            }
            else 
            {
                SetUpDoubleProfitUI();
            }
        }

        private void DoubleProfitOn()
        {
            timer.SetActive(true);
            doubleProfitSaveData.TurnOnDoubleProfit();
            SaveDoubleProfit();
            isDoubleProfitOn = true;
            DoubleProfitMultiplier = 2;
        }

        private void DoubleProfitOff()
        {
            timer.SetActive(false);
            doubleProfitSaveData.TurnOffDoubleProfit();
            SetUpDoubleProfitUI();
            SaveDoubleProfit();
            isDoubleProfitOn = false;
            DoubleProfitMultiplier = 1;
        }

        private void StartDoubleProfit(int currentDuration)
        {
            KillDoubleProfitTweenTimer();

            if (!isDoubleProfitOn)
            {
                DoubleProfitOn();
                doubleProfitTimerText.text = DBExtension.GetFormattedTimeSpan(currentDuration);
            }
            else
            {
                doubleProfitSaveData.CurrentDoubleProfitDuration += currentDuration;
                currentDuration = doubleProfitSaveData.CurrentDoubleProfitDuration;
                SaveDoubleProfit();
            }    

            int remainingDuration = currentDuration;

            DOTween.To(() => remainingDuration, x => remainingDuration = x, 0, remainingDuration)
                .SetEase(Ease.Linear).SetId(DBTweenTypes.DoubleProfit + PROFIT_TWEEN)
                .OnUpdate(() =>
                {
                    if (doubleProfitSaveData.CurrentDoubleProfitDuration != remainingDuration)
                    {
                        doubleProfitSaveData.CurrentDoubleProfitDuration = remainingDuration;
                        SaveDoubleProfit();
                    }
                    doubleProfitTimerText.text = DBExtension.GetFormattedTimeSpan(remainingDuration);
                })
                .OnComplete(() => DoubleProfitOff());
        }

        private void StartDoubleProfitAfterPause(int durationAfterPause)
        {
            KillDoubleProfitTweenTimer();
            DoubleProfitOn();

            int remainingDuration = durationAfterPause;

            DOTween.To(() => remainingDuration, x => remainingDuration = x, 0, remainingDuration)
                .SetEase(Ease.Linear).SetId(DBTweenTypes.DoubleProfit + PROFIT_TWEEN)
                .OnUpdate(() =>
                {
                    if (doubleProfitSaveData.CurrentDoubleProfitDuration != remainingDuration)
                    {
                        doubleProfitSaveData.CurrentDoubleProfitDuration = remainingDuration;
                        SaveDoubleProfit();
                    }
                    doubleProfitTimerText.text = DBExtension.GetFormattedTimeSpan(remainingDuration);
                })
                .OnComplete(() => DoubleProfitOff());
        }

        public void ActivateDoubleProfit()
        {
            DBExtension.WatchAd();
            StartDoubleProfit(doubleProfitSaveData.DoubleProfitDuration);
        }

        private void LoadDoubleProfit() =>
            DBManager.Instance.SaveManager.Load((DoubleProfitData data) =>
            doubleProfitSaveData = data ?? new DoubleProfitData(false, 1, 300, 0));

        public void SaveDoubleProfit()
        {
            doubleProfitSaveData.CurrentDoubleProfitDuration += (int)(DateTime.Now - doubleProfitSaveData.LastSavedTime).TotalSeconds;
            doubleProfitSaveData.LastSavedTime = DateTime.Now;
            DBManager.Instance.SaveManager.Save(doubleProfitSaveData);
        }

        private void SetUpDoubleProfitUI() => doubleProfitTimerText.text = DBExtension.GetFormattedTimeSpan(doubleProfitSaveData.DoubleProfitDuration);

        private int DoubleProfitTimeLeftAfterPause(int currentDuration)
        {
            int offlineTime = Manager.TimerManager.GetLastOfflineTimeSeconds();
            int remainingTime = currentDuration - offlineTime;

            if (remainingTime < 0)
            {
                remainingTime = 0;
            }

            return remainingTime;
        }

        private void DoubleProfitPauseCheck(object pauseStatus)
        {
            bool isPaused = (bool)pauseStatus;

            if (isPaused)
            {
                KillDoubleProfitTweenTimer();
            }
            else if (isDoubleProfitOn)
            {
                ContinueDoubleProfitAfterPause();
            }
        }

        private void ContinueDoubleProfitAfterPause()
        {
            int currentTime = DoubleProfitTimeLeftAfterPause(doubleProfitSaveData.CurrentDoubleProfitDuration);
            doubleProfitTimerText.text = DBExtension.GetFormattedTimeSpan(currentTime);
            doubleProfitSaveData.CurrentDoubleProfitDuration = currentTime;
            SaveDoubleProfit();

            StartDoubleProfitAfterPause(currentTime);
        }

        private void KillDoubleProfitTweenTimer() => DOTween.Kill(DBTweenTypes.DoubleProfit + PROFIT_TWEEN);
    }
}
