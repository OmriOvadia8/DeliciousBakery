using UnityEngine;
using DB_Core;
using System;

namespace DB_Game
{
    public class DBCurrencyManager : DBLogicMonoBehaviour
    {
        public int startingCurrency = 0;
        public CurrencySaveData currencySaveData;

        private void Awake()
        {
            LoadCurrency();
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

        public void SaveCurrency() // saving the current player's currency
        {
            DBManager.Instance.SaveManager.Save(currencySaveData);
        }

        public void LoadCurrency() // loading the latest player's currency and if there is no save then starts at 0
        {
            DBManager.Instance.SaveManager.Load<CurrencySaveData>(delegate (CurrencySaveData data)
            {
                currencySaveData = data ?? new CurrencySaveData(0);
            });
        }

        [Serializable]
        public class CurrencySaveData : IDBSaveData
        {
            public int CurrencyAmount;

            public CurrencySaveData(int currencyAmount)
            {
                CurrencyAmount = currencyAmount;
            }
        }
    }
}
