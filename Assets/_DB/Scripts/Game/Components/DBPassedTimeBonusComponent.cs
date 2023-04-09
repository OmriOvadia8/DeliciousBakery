using DB_Core;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace DB_Game
{
    public class DBPassedTimeBonusComponent : DBLogicMonoBehaviour
    {
        [SerializeField] private TMP_Text rewardText;
        [SerializeField] private float xOffsetPerDigit = 10f;
        [SerializeField] private RectTransform coinRectTransform;
        [SerializeField] Button gotItButton;
        [SerializeField] Button claimButton;

        private int totalReturnBonus = 0;

        private float initialXPos;

        private void Awake()
        {
            initialXPos = coinRectTransform.anchoredPosition.x;
        }

        private void Start()
        {
            OpenOfflineRewardWindow(Manager.TimerManager.GetLastOfflineTimeSeconds());

            Manager.EventsManager.AddListener(DBEventNames.OfflineTimeRefreshed, OnRefreshedTime);

            if(totalReturnBonus == 0)
            {
                gotItButton.gameObject.SetActive(true);
                claimButton.gameObject.SetActive(false);
            }
            else
            {
                gotItButton.gameObject.SetActive(false);
                claimButton.gameObject.SetActive(true);
            }
        }

        private void OnDestroy()
        {
            Manager.EventsManager.RemoveListener(DBEventNames.OfflineTimeRefreshed, OnRefreshedTime);
        }

        private void OnRefreshedTime(object timePassed)
        {
            OpenOfflineRewardWindow((int)timePassed);
        }

        public void GivePassiveBonusAccordingToTimePassed()
        {
            GameLogic.ScoreManager.ChangeScoreByTagByAmount(ScoreTags.GameCurrency, totalReturnBonus);
            InvokeEvent(DBEventNames.CurrencyUpdateUI, null);
        }

        public void GiveDoubleBonusAccordingToTimePassed()
        {
            GivePassiveBonusAccordingToTimePassed();

            if (totalReturnBonus > 0)
            {
                WatchAd();
            }
        }

        private void OpenOfflineRewardWindow(int timePassed)
        {
            totalReturnBonus = DBPauseCurrencyManager.PassedTimeFoodRewardCalc(timePassed);

            rewardText.text = totalReturnBonus.ToString();
            float xPos = initialXPos - (totalReturnBonus.ToString().Length - 1) * xOffsetPerDigit;
            coinRectTransform.anchoredPosition = new Vector2(xPos, coinRectTransform.anchoredPosition.y);

            GivePassiveBonusAccordingToTimePassed();

            DBDebug.LogException("Offline WINDOW reward : " + totalReturnBonus);
        }

        public void WatchAd()
        {
            DBManager.Instance.AdsManager.ShowAd(null);
        }
    }
}