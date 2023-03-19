using Core;
using UnityEngine;
using TMPro;

namespace Game
{
    public class HOGPassedTimeBonusComponent : HOGLogicMonoBehaviour
    {
        [SerializeField] HOGMoneyHolder playerCurrency;
        [SerializeField] private int rewardPerSecond = 10;
        [SerializeField] private int maxReward = 5000;
        [SerializeField] private TMP_Text rewardText;
        [SerializeField] private float xOffsetPerDigit = 10f;
        [SerializeField] private RectTransform coinRectTransform;

        private int returnBonus = 0;

        private float initialXPos;

        private void Awake()
        {
            initialXPos = coinRectTransform.anchoredPosition.x;
        }

        private void Start()
        {
            OpenOfflineRewardWindow(Manager.TimerManager.GetLastOfflineTimeSeconds());

            Manager.EventsManager.AddListener(HOGEventNames.OfflineTimeRefreshed, OnRefreshedTime);
        }

        private void OnDestroy()
        {
            Manager.EventsManager.RemoveListener(HOGEventNames.OfflineTimeRefreshed, OnRefreshedTime);
        }

        private void OnRefreshedTime(object timePassed)
        {
            OpenOfflineRewardWindow((int)timePassed);
        }

        public void GivePassiveBonusAccordingToTimePassed()
        {
            GameLogic.ScoreManager.ChangeScoreByTagByAmount(ScoreTags.GameCurrency, returnBonus);
            playerCurrency.UpdateCurrency(playerCurrency.startingCurrency);
        }

        private void OpenOfflineRewardWindow(int timePassed)
        {
            returnBonus = timePassed / rewardPerSecond;
            rewardText.text = returnBonus.ToString();

            if (returnBonus > 0)
            {
                this.gameObject.SetActive(true);
                returnBonus = Mathf.Min(returnBonus, maxReward);
                GivePassiveBonusAccordingToTimePassed();
            }

            else
            {
                HideWindow();
            }

            float xPos = initialXPos - (returnBonus.ToString().Length - 1) * xOffsetPerDigit;
            coinRectTransform.anchoredPosition = new Vector2(xPos, coinRectTransform.anchoredPosition.y);
        }

        public void HideWindow()
        {
            this.gameObject.SetActive(false);
        }
    }
}