using DB_Core;
using System;
using UnityEngine;
using TMPro;
using DG.Tweening;

namespace DB_Game
{
    public class DoubleProfitComponent : DBLogicMonoBehaviour
    {
        private static bool isDoubleProfitOn = false;
        public static int doubleProfitMultiplier = 1;
        public DoubleProfitData doubleProfitSaveData;
        [SerializeField] TMP_Text doubleProfitTimerText;

        private void Awake()
        {
            LoadDoubleProfit();
        }

        private void OnEnable()
        {
            AddListener(DBEventNames.OnPause, DoubleProfitPauseCheck);
        }

        private void Start()
        {
            if (doubleProfitSaveData.IsDoubleProfitOn && Manager.TimerManager.GetLastOfflineTimeSeconds() > 0)
            {
                ContinueDoubleProfitAfterPause();
            }

            else 
            {
                SetUpDoubleProfitUI();
            }
        }

        private void OnDisable()
        {
            RemoveListener(DBEventNames.OnPause, DoubleProfitPauseCheck);
        }

        public void DoubleProfitOn()
        {
            doubleProfitSaveData.TurnOnDoubleProfit();
            SaveDoubleProfit();
            isDoubleProfitOn = true;
            doubleProfitMultiplier = 2;
        }

        public void DoubleProfitOff()
        {
            doubleProfitSaveData.TurnOffDoubleProfit();
            SetUpDoubleProfitUI();
            SaveDoubleProfit();
            isDoubleProfitOn = false;
            doubleProfitMultiplier = 1;
        }

        private void StartDoubleProfit(int currentDuration)
        {
            KillDoubleProfitTweenTimer();

            if (!isDoubleProfitOn)
            {
                DoubleProfitOn();
                doubleProfitTimerText.text = TimeSpan.FromSeconds(currentDuration).ToString("mm':'ss");
            }

            else
            {
                doubleProfitSaveData.CurrentDoubleProfitDuration += currentDuration;
                currentDuration = doubleProfitSaveData.CurrentDoubleProfitDuration;
                SaveDoubleProfit();
            }

            int remainingDuration = currentDuration;

            DOTween.To(() => remainingDuration, x => remainingDuration = x, 0, remainingDuration)
                .SetEase(Ease.Linear)
                .SetId(DBTweenTypes.DoubleProfit)
                .OnUpdate(() =>
                {
                    if (doubleProfitSaveData.CurrentDoubleProfitDuration != remainingDuration)
                    {
                        doubleProfitSaveData.CurrentDoubleProfitDuration = remainingDuration;
                        SaveDoubleProfit();
                    }
                    doubleProfitTimerText.text = TimeSpan.FromSeconds(remainingDuration).ToString("mm':'ss");
                })
                .OnComplete(() =>
                {
                    DoubleProfitOff();
                });
        }

        private void StartDoubleProfitAfterPause(int durationAfterPause)
        {
            KillDoubleProfitTweenTimer();
            DoubleProfitOn();

            int remainingDuration = durationAfterPause;

            DOTween.To(() => remainingDuration, x => remainingDuration = x, 0, remainingDuration)
                .SetEase(Ease.Linear)
                .SetId(DBTweenTypes.DoubleProfit)
                .OnUpdate(() =>
                {
                    if (doubleProfitSaveData.CurrentDoubleProfitDuration != remainingDuration)
                    {
                        doubleProfitSaveData.CurrentDoubleProfitDuration = remainingDuration;
                        SaveDoubleProfit();
                    }
                    doubleProfitTimerText.text = TimeSpan.FromSeconds(remainingDuration).ToString("mm':'ss");
                })
                .OnComplete(() =>
                {
                    DoubleProfitOff();
                });
        }

        public void ActivateDoubleProfit()
        {
            DBExtension.WatchAd();
            StartDoubleProfit(doubleProfitSaveData.DoubleProfitDuration);
        }

        private void LoadDoubleProfit()
        {
            DBManager.Instance.SaveManager.Load<DoubleProfitData>(delegate (DoubleProfitData data)
            {
                doubleProfitSaveData = data ?? new DoubleProfitData(false, 1, 5, 0);
            });
        }

        public void SaveDoubleProfit()
        {
            doubleProfitSaveData.CurrentDoubleProfitDuration += (int)(DateTime.Now - doubleProfitSaveData.LastSavedTime).TotalSeconds;
            doubleProfitSaveData.LastSavedTime = DateTime.Now;
            DBManager.Instance.SaveManager.Save(doubleProfitSaveData);
        }

        private void SetUpDoubleProfitUI()
        {
            doubleProfitTimerText.text = TimeSpan.FromSeconds(doubleProfitSaveData.DoubleProfitDuration).ToString("mm':'ss");
        }

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

        private void DoubleProfitPauseCheck(object isPaused)
        {
            if (!(bool)isPaused)
            {
                if (isDoubleProfitOn)
                {
                   ContinueDoubleProfitAfterPause();
                }
            }

            else
            {
                KillDoubleProfitTweenTimer();
            }
        }

        private void ContinueDoubleProfitAfterPause()
        {
            int currentTime = DoubleProfitTimeLeftAfterPause(doubleProfitSaveData.CurrentDoubleProfitDuration);
            doubleProfitTimerText.text = TimeSpan.FromSeconds(currentTime).ToString("mm':'ss");
            doubleProfitSaveData.CurrentDoubleProfitDuration = currentTime;
            SaveDoubleProfit();

            StartDoubleProfitAfterPause(currentTime);
        }

        private void KillDoubleProfitTweenTimer()
        {
            DOTween.Kill(DBTweenTypes.DoubleProfit);
        }
    }

    [Serializable]
    public class DoubleProfitData : IDBSaveData
    {
        public bool IsDoubleProfitOn;
        public int DoubleProfitMultiplier;
        public int DoubleProfitDuration;
        public int CurrentDoubleProfitDuration;
        public DateTime LastSavedTime;

        public DoubleProfitData(bool isDouble, int doubleAmount, int doubleProfitDuration, int currentProfitDuration)
        {
            IsDoubleProfitOn = isDouble;
            DoubleProfitMultiplier = doubleAmount;
            DoubleProfitDuration = doubleProfitDuration;
            CurrentDoubleProfitDuration = currentProfitDuration;
            LastSavedTime = DateTime.Now;
        }
        public void TurnOnDoubleProfit()
        {
            IsDoubleProfitOn = true;
            DoubleProfitMultiplier = 2;
        }

        public void TurnOffDoubleProfit()
        {
            IsDoubleProfitOn = false;
            DoubleProfitMultiplier = 1;
            DoubleProfitDuration = 5;
        }
    }
}
