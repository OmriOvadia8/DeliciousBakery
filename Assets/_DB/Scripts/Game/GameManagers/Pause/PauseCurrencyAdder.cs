using DB_Core;
using UnityEngine;

namespace DB_Game
{
    public class PauseCurrencyAdder : DBLogicMonoBehaviour
    {
        private double pausedReward;

        [SerializeField] DBPauseCurrencyManager pausedCurrencyManager;

        private void OnEnable() => AddListener(DBEventNames.OfflineTimeRefreshed, OnPausedEarning);

        private void OnDisable() => RemoveListener(DBEventNames.OfflineTimeRefreshed, OnPausedEarning);

        private void OnPausedEarning(object timePassed)
        {
            pausedReward = 0;
            pausedReward = pausedCurrencyManager.PassedTimeFoodRewardCalc((int)timePassed);

            GameLogic.ScoreManager.ChangeScoreByTagByAmount(ScoreTags.GameCurrency, pausedReward);
            InvokeEvent(DBEventNames.CurrencyUpdateUI, null);
        }
    }
}