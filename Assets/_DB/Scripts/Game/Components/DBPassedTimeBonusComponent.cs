using DB_Core;
using UnityEngine;
using TMPro;

namespace DB_Game
{
    public class DBPassedTimeBonusComponent : DBLogicMonoBehaviour
    {
        [SerializeField] private int rewardPerSecond = 300;
        [SerializeField] int baseMaxReward = 1500;
        [SerializeField] private TMP_Text rewardText;
        [SerializeField] private float xOffsetPerDigit = 10f;
        [SerializeField] private RectTransform coinRectTransform;

        private int[] maxFoodReward;
        private int[] returnFoodBonus;

        private int totalFoodBonus = 0;
        public int totalReturnBonus = 0;

        private float initialXPos;

        private void Awake()
        {
            initialXPos = coinRectTransform.anchoredPosition.x;
        }

        private void Start()
        {
            returnFoodBonus = new int[DBFoodManager.FOOD_COUNT];
            maxFoodReward = new int[DBFoodManager.FOOD_COUNT];

            OpenOfflineRewardWindow(Manager.TimerManager.GetLastOfflineTimeSeconds());

            Manager.EventsManager.AddListener(DBEventNames.OfflineTimeRefreshed, OnRefreshedTime);
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
            GameLogic.ScoreManager.ChangeScoreByTagByAmount(ScoreTags.GameCurrency, totalReturnBonus);
            WatchAd();
            InvokeEvent(DBEventNames.CurrencyUpdateUI, null);
        }

        private void OpenOfflineRewardWindow(int timePassed)
        {
            totalReturnBonus = PassedTimeFoodRewardCalc(timePassed);

            rewardText.text = totalReturnBonus.ToString();
            float xPos = initialXPos - (totalReturnBonus.ToString().Length - 1) * xOffsetPerDigit;
            coinRectTransform.anchoredPosition = new Vector2(xPos, coinRectTransform.anchoredPosition.y);

            if (totalReturnBonus > 1000)
            {
                GivePassiveBonusAccordingToTimePassed();
            }

            else
            {
                return;
            }
}

        public int PassedTimeFoodRewardCalc(int timePassed)
        {
            totalFoodBonus = 0;
            totalReturnBonus = 0;

            for (int i = 0; i < DBFoodManager.FOOD_COUNT; i++)
            {
                var foodID = DBFoodManager.GetFoodData(i);
                returnFoodBonus[i] = 0;

                if(foodID.IsFoodLocked == false && foodID.IsIdleFood == true)
                {
                    returnFoodBonus[i] = (int)((timePassed / (foodID.BakerCookingTime) 
                        * foodID.CookFoodTimes) * foodID.Profit * 0.1f); // 10% of offline time divided by baker cooking time for the specific food and the amount of time he bakes it * its profit 
                    maxFoodReward[i] = baseMaxReward * foodID.CookFoodTimes; // capacity for reward so it won't be exploit
                    returnFoodBonus[i] = Mathf.Min(returnFoodBonus[i], maxFoodReward[i]);

                    totalFoodBonus += returnFoodBonus[i];
                }
            }

            return totalFoodBonus + (timePassed / rewardPerSecond); // extra reward mainly for beginners to get easier start (barely noticeable later on)
        }

        public void WatchAd()
        {
            DBManager.Instance.AdsManager.ShowAd(null);
        }
    }
}