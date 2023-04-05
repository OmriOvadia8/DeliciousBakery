using DB_Core;
using UnityEngine;

namespace DB_Game
{
    public class DBUpgradeIntervalComponent : DBLogicMonoBehaviour
    {
        [SerializeField] private int scoreUpAmount;
        [SerializeField] private ScoreTags scoreTag;
        [SerializeField] private int intervalTime;

        private void OnEnable()
        {
            Manager.TimerManager.SubscribeTimer(intervalTime, ChangeScore);
        }

        private void OnDisable()
        {
            Manager.TimerManager.UnSubscribeTimer(intervalTime, ChangeScore);
        }

        private void ChangeScore()
        {
            GameLogic.ScoreManager.ChangeScoreByTagByAmount(scoreTag, scoreUpAmount);

            var scoreText = (DBTweenMoneyComponent)Manager.PoolManager.GetPoolable(PoolNames.MoneyToast);
            scoreText.transform.position = transform.position;
            scoreText.Init(scoreUpAmount);
        }
    }
}