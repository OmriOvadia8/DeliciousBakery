using Core;
using UnityEngine;
using TMPro;

namespace Game
{
    public class HOGPassedTimeBonusComponent : HOGLogicMonoBehaviour
    {
        [SerializeField] private int rewardPerSecond = 10;
        [SerializeField] private int maxReward = 5000;
        [SerializeField] private TMP_Text rewardText;

        private void Start()
        {
            GiveBonusAccordingToTimePassed(Manager.TimerManager.GetLastOfflineTimeSeconds());

            Manager.EventsManager.AddListener(HOGEventNames.OfflineTimeRefreshed, OnRefreshedTime);
        }

        private void OnDestroy()
        {
            Manager.EventsManager.RemoveListener(HOGEventNames.OfflineTimeRefreshed, OnRefreshedTime);
        }

        private void OnRefreshedTime(object timePassed)
        {
            GiveBonusAccordingToTimePassed((int)timePassed);
        }

        private void GiveBonusAccordingToTimePassed(int timePassed)
        {
            var returnBonus = timePassed / rewardPerSecond;
            rewardText.text = returnBonus.ToString();

            if (returnBonus > 0)
            {
                this.gameObject.SetActive(true);
                returnBonus = Mathf.Min(returnBonus, maxReward);
                GameLogic.ScoreManager.ChangeScoreByTagByAmount(ScoreTags.GameCurrency, returnBonus);
            }
            else
            {
                HideWindow();
            }
        }

        public void HideWindow()
        {
            this.gameObject.SetActive(false);
        }
    }
}