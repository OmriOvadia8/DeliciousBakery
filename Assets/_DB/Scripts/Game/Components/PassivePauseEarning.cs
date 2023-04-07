using DB_Core;

namespace DB_Game
{
    public class PassivePauseEarning : DBPassedTimeBonusComponent
    {
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
            GivePassiveEarningAfterPausing((int)timePassed);
        }

        private void GivePassiveEarningAfterPausing(int timePassed)
        {
            totalReturnBonus = PassedTimeFoodRewardCalc(timePassed);

            if (totalReturnBonus > 1000)
            {
                GivePassiveBonusAccordingToTimePassed();
            }

            else
            {
                return;
            }
        }
    }
}