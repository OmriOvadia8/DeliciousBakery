using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DB_Core
{
    public class DBTimeManager
    {
        private bool isLooping;

        private Dictionary<int, List<DBTimerData>> timerActions = new();
        private List<DBAlarmData> activeAlarms = new();

        private DBOfflineTime dbOfflineTime;

        private int counter;
        private int alarmCounter;
        private int offlineSeconds;

        public DBTimeManager()
        {
            _ = TimerLoop();

            DBManager.Instance.SaveManager.Load((DBOfflineTime data) =>
            {
                dbOfflineTime = data ?? new DBOfflineTime
                {
                    LastCheck = DateTime.UtcNow,
                };

                DBManager.Instance.SaveManager.Save(dbOfflineTime);
                CheckOfflineTime();
            });

            DBManager.Instance.EventsManager.AddListener(DBEventNames.OnPause, OnPause);
        }

        private void OnPause(object pauseStatus) => CheckOfflineTime();

        ~DBTimeManager()
        {
            isLooping = false;
            DBManager.Instance.EventsManager.RemoveListener(DBEventNames.OnPause, OnPause);
        }

        private void CheckOfflineTime()
        {
            var timePassed = DateTime.UtcNow - dbOfflineTime.LastCheck;
            offlineSeconds = (int)timePassed.TotalSeconds;
            dbOfflineTime.LastCheck = DateTime.UtcNow;
            DBManager.Instance.SaveManager.Save(dbOfflineTime);
            DBDebug.Log(offlineSeconds);
            DBManager.Instance.EventsManager.InvokeEvent(DBEventNames.OfflineTimeRefreshed, offlineSeconds);
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

                if (DateTime.Compare(alarmData.AlarmTime, DateTime.UtcNow) < 0)
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
                timerActions.Add(intervalSeconds, new List<DBTimerData>());
            }

            timerActions[intervalSeconds].Add(new DBTimerData(counter, onTickAction));
        }

        public void UnSubscribeTimer(int intervalSeconds, Action onTickAction)
        {
            timerActions[intervalSeconds].RemoveAll(x => x.TimerAction == onTickAction);
        }

        public int SetAlarm(int seconds, Action onAlarmAction)
        {
            alarmCounter++;

            var alarmData = new DBAlarmData
            {
                ID = alarmCounter,
                AlarmTime = DateTime.UtcNow.AddSeconds(seconds),
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
            if (!dbOfflineTime.LeftOverTimes.ContainsKey(timeType))
            {
                return 0;
            }

            return dbOfflineTime.LeftOverTimes[timeType];
        }

        public void SetLeftOverTime(OfflineTimes timeType, int timeAmount) => dbOfflineTime.LeftOverTimes[timeType] = timeAmount;
    }

    public class DBTimerData
    {
        public Action TimerAction;
        public int StartCounter;

        public DBTimerData(int counter, Action onTickAction)
        {
            TimerAction = onTickAction;
            StartCounter = counter;
        }
    }

    public class DBAlarmData
    {
        public int ID;
        public DateTime AlarmTime;
        public Action AlarmAction;
    }

    [Serializable]
    public class DBOfflineTime : IDBSaveData
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