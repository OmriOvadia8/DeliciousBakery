using TMPro;
using UnityEngine;

namespace DB_Game
{
    public class DBTimerComponent : DBLogicMonoBehaviour
    {
        [SerializeField] private TMP_Text timerText;
        private int timer = 0;

        private void OnEnable()
        {
            Manager.TimerManager.SubscribeTimer(1, SetTimer);
        }

        private void OnDisable()
        {
            Manager.TimerManager.UnSubscribeTimer(1, SetTimer);
        }

        private void SetTimer()
        {
            timer++;
            timerText.SetText(timer.ToString("00:00"));
        }
    }
}