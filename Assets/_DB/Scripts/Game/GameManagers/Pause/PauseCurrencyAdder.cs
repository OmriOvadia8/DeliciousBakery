using DB_Core;
using UnityEngine;

namespace DB_Game
{
    public class PauseCurrencyAdder : DBLogicMonoBehaviour
    {
        private int pausedReward;
        [SerializeField] DBPauseCurrencyManager pausedCurrencyManager;

        private void Start()
        {
            Manager.EventsManager.AddListener(DBEventNames.OfflineTimeRefreshed, OnPausedEarning);
        }

        private void OnDestroy()
        {
            Manager.EventsManager.RemoveListener(DBEventNames.OfflineTimeRefreshed, OnPausedEarning);
        }

        private void OnPausedEarning(object timePassed)
        {
            pausedReward = 0;
            pausedReward = pausedCurrencyManager.PassedTimeFoodRewardCalc((int)timePassed);

            GameLogic.ScoreManager.ChangeScoreByTagByAmount(ScoreTags.GameCurrency, pausedReward);
            Manager.EventsManager.InvokeEvent(DBEventNames.CurrencyUpdateUI, null);

        }
    }
}