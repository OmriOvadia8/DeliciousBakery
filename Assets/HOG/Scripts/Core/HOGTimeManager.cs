using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core
{
    public class HOGTimeManager
    {
        private bool isLooping = false;

        private Dictionary<int, List<HOGTimerData>> timerActions = new();
        private List<HOGAlarmData> activeAlarms = new();

        private HOGOfflineTime hogOfflineTime;

        private int counter;
        private int alarmCounter;
        private int offlineSeconds;

        public HOGTimeManager()
        {
            _ = TimerLoop();

            HOGManager.Instance.SaveManager.Load(delegate (HOGOfflineTime data)
            {
                hogOfflineTime = data ?? new HOGOfflineTime
                {
                    LastCheck = DateTime.Now
                };

                HOGManager.Instance.SaveManager.Save(hogOfflineTime);
                CheckOfflineTime();
            });

            HOGManager.Instance.EventsManager.AddListener(HOGEventNames.OnPause, OnPause);
        }

        private void OnPause(object pauseStatus)
        {
            CheckOfflineTime();
        }

        ~HOGTimeManager()
        {
            isLooping = false;
            HOGManager.Instance.EventsManager.RemoveListener(HOGEventNames.OnPause, OnPause);
        }

        private void CheckOfflineTime()
        {
            var timePassed = DateTime.Now - hogOfflineTime.LastCheck;
            offlineSeconds = (int)timePassed.TotalSeconds;
            hogOfflineTime.LastCheck = DateTime.Now;
            HOGManager.Instance.SaveManager.Save(hogOfflineTime);

            HOGDebug.LogException($"Last offline time is {offlineSeconds}");

            HOGManager.Instance.EventsManager.InvokeEvent(HOGEventNames.OfflineTimeRefreshed, offlineSeconds);
        }

        public int GetLastOfflineTimeSeconds()
        {
            return offlineSeconds;
        }

        private async Task TimerLoop()
        {
            isLooping = true;

            while (isLooping)
            {
                await Task.Delay(1000);
                InvokeTime();
            }

            isLooping = false;
        }

        private void InvokeTime()
        {
            counter++;

            foreach (var timers in timerActions)
            {
                foreach (var timer in timers.Value)
                {
                    var offsetCounter = counter - timer.StartCounter;

                    if (offsetCounter % timers.Key == 0)
                    {
                        timer.TimerAction.Invoke();
                    }
                }
            }

            for (var index = 0; index < activeAlarms.Count; index++)
            {
                var alarmData = activeAlarms[index];

                if (DateTime.Compare(alarmData.AlarmTime, DateTime.Now) < 0)
                {
                    alarmData.AlarmAction.Invoke();
                    activeAlarms.Remove(alarmData);
                }
            }
        }

        public void SubscribeTimer(int intervalSeconds, Action onTickAction)
        {
            if (!timerActions.ContainsKey(intervalSeconds))
            {
                timerActions.Add(intervalSeconds, new List<HOGTimerData>());
            }

            timerActions[intervalSeconds].Add(new HOGTimerData(counter, onTickAction));
        }

        public void UnSubscribeTimer(int intervalSeconds, Action onTickAction)
        {
            timerActions[intervalSeconds].RemoveAll(x => x.TimerAction == onTickAction);
        }

        public int SetAlarm(int seconds, Action onAlarmAction)
        {
            alarmCounter++;

            var alarmData = new HOGAlarmData
            {
                ID = alarmCounter,
                AlarmTime = DateTime.Now.AddSeconds(seconds),
                AlarmAction = onAlarmAction
            };

            activeAlarms.Add(alarmData);
            return alarmCounter;
        }

        public void DisableAlarm(int alarmID)
        {
            activeAlarms.RemoveAll(x => x.ID == alarmID);
        }

        public int GetLeftOverTime(OfflineTimes timeType)
        {
            if (!hogOfflineTime.LeftOverTimes.ContainsKey(timeType))
            {
                return 0;
            }

            return hogOfflineTime.LeftOverTimes[timeType];
        }

        public void SetLeftOverTime(OfflineTimes timeType, int timeAmount)
        {
            hogOfflineTime.LeftOverTimes[timeType] = timeAmount;
        }
    }

    public class HOGTimerData
    {
        public Action TimerAction;
        public int StartCounter;

        public HOGTimerData(int counter, Action onTickAction)
        {
            TimerAction = onTickAction;
            StartCounter = counter;
        }
    }

    public class HOGAlarmData
    {
        public int ID;
        public DateTime AlarmTime;
        public Action AlarmAction;
    }

    [Serializable]
    public class HOGOfflineTime : IHOGSaveData
    {
        public DateTime LastCheck;
        public Dictionary<OfflineTimes, int> LeftOverTimes = new();
    }

    public enum OfflineTimes
    {
        DailyBonus,
        ExtraBonus
    }
}