using UnityEngine;
using DB_Core;
using System;

namespace DB_Game
{
    public class DBCurrencyManager : DBLogicMonoBehaviour
    {
        public int startingCurrency = 0;
        public int startingPremCurrency = 0;
        public CurrencySaveData currencySaveData;

        private void Awake()
        {
            LoadCurrency();
        }

        private void OnEnable()
        {
            AddListener(DBEventNames.CurrencyUpdateUI, UpdateCurrencyAfterScoreChange);
            AddListener(DBEventNames.PremCurrencyUpdateUI, UpdatePremCurrencyAfterScoreChange);
        }

        private void OnDisable()
        {
            AddListener(DBEventNames.CurrencyUpdateUI, UpdateCurrencyAfterScoreChange);
            RemoveListener(DBEventNames.PremCurrencyUpdateUI, UpdatePremCurrencyAfterScoreChange);
        }

        public void UpdateCurrency(int foodProfit)
        {
            GameLogic.ScoreManager.ChangeScoreByTagByAmount(ScoreTags.GameCurrency, foodProfit);
            if (DBGameLogic.Instance.ScoreManager.TryGetScoreByTag(ScoreTags.GameCurrency, ref currencySaveData.CurrencyAmount))
            {
                InvokeEvent(DBEventNames.OnCurrencySet, currencySaveData.CurrencyAmount);
                SaveCurrency();
            }
        }

        private void UpdatePremCurrency(int starIncrease)
        {
            GameLogic.ScoreManager.ChangeScoreByTagByAmount(ScoreTags.PremiumCurrency, starIncrease);
            if (DBGameLogic.Instance.ScoreManager.TryGetScoreByTag(ScoreTags.PremiumCurrency, ref currencySaveData.PremCurrencyAmount))
            {
                InvokeEvent(DBEventNames.OnPremCurrencySet, currencySaveData.PremCurrencyAmount);
                SaveCurrency();
            }
        }
 
        public void TestPrem()
        {
            UpdatePremCurrency(1000);
        }

        public void UsePrem()
        {
            DBGameLogic.Instance.ScoreManager.TryUseScore(ScoreTags.PremiumCurrency, 1000);
            UpdatePremCurrency(0);
        }

        public void SaveCurrency() // saving the current player's currency
        {
            DBManager.Instance.SaveManager.Save(currencySaveData);
        }

        public void LoadCurrency() // loading the latest player's currency and if there is no save then starts at 0
        {
            DBManager.Instance.SaveManager.Load<CurrencySaveData>(delegate (CurrencySaveData data)
            {
                currencySaveData = data ?? new CurrencySaveData(0, 0);
            });
        }

        private void UpdateCurrencyAfterScoreChange(object obj)
        {
            UpdateCurrency(startingCurrency);
        }

        private void UpdatePremCurrencyAfterScoreChange(object obj)
        {
            UpdatePremCurrency(startingPremCurrency);
        }

        [Serializable]
        public class CurrencySaveData : IDBSaveData
        {
            public int CurrencyAmount;
            public int PremCurrencyAmount;

            public CurrencySaveData(int currencyAmount, int premCurrencyAmount)
            {
                CurrencyAmount = currencyAmount;
                PremCurrencyAmount = premCurrencyAmount;
            }
        }
    }
}
