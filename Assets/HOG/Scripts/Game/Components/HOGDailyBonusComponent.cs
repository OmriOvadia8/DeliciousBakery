using Core;
using UnityEngine;

namespace Game
{
    //TODO: Move to manager, this should be UI only
    public class HOGDailyBonusComponent : HOGLogicMonoBehaviour
    {
        [SerializeField] private int hours = 24;

        private void Start()
        {
            var lastOfflineTimeSeconds = Manager.TimerManager.GetLastOfflineTimeSeconds();
            var leftOverTime = Manager.TimerManager.GetLeftOverTime(OfflineTimes.DailyBonus);

            var totalSeconds = lastOfflineTimeSeconds + leftOverTime;

            if (totalSeconds >= hours.HoursToSeconds())
            {
                GameLogic.ScoreManager.ChangeScoreByTagByAmount(ScoreTags.GameCurrency, 1000);
                var leftOver = totalSeconds - hours.HoursToSeconds();
                Manager.TimerManager.SetLeftOverTime(OfflineTimes.DailyBonus, Mathf.Min(leftOver / 2, leftOver));
            }
            else
            {
                Manager.TimerManager.SetLeftOverTime(OfflineTimes.DailyBonus, totalSeconds);
            }
        }
    }
}