using DB_Core;
using UnityEngine;

namespace DB_Game
{
    public class PauseCurrencyAdder : DBLogicMonoBehaviour
    {
        private double pausedReward;
        private const int NERF_REWARD = 4;  
        [SerializeField] DBPauseCurrencyManager pausedCurrencyManager;

        private void OnEnable() => AddListener(DBEventNames.OfflineTimeRefreshed, OnPausedEarning);

        private void OnDisable() => RemoveListener(DBEventNames.OfflineTimeRefreshed, OnPausedEarning);

        private void OnPausedEarning(object timePassed)
        {
            pausedReward = 0;
            pausedReward = pausedCurrencyManager.PassedTimeFoodRewardCalc((int)timePassed) / NERF_REWARD;

            GameLogic.ScoreManager.ChangeScoreByTagByAmount(ScoreTags.GameCurrency, pausedReward);
            InvokeEvent(DBEventNames.CurrencyUpdateUI, null);
            InvokeEvent(DBEventNames.BuyButtonsCheck, null);
        }
    }
}