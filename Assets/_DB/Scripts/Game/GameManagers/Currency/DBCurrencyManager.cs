using DB_Core;
using System;

namespace DB_Game
{
    public class DBCurrencyManager : DBLogicMonoBehaviour
    {
        public int initialCoinsAmount = 0;
        public int initialStarsAmount = 0;
        public CurrencySaveData currencySaveData;

        private void Awake() => LoadCurrency();

        private void OnEnable() => RegisterEvents();

        private void OnDisable() => UnregisterEvents();

        public void CoinsEarn(object foodProfit)
        {
            int profit = (int)foodProfit;
            GameLogic.ScoreManager.ChangeScoreByTagByAmount(ScoreTags.GameCurrency, profit);

            if (DBGameLogic.Instance.ScoreManager.TryGetScoreByTag(ScoreTags.GameCurrency, ref currencySaveData.CoinsAmount))
            {
                InvokeEvent(DBEventNames.OnCurrencySet, currencySaveData.CoinsAmount);
                SaveCurrency();
            }
        }

        private void UpdateStarsCurrency(int starIncrease)
        {
            GameLogic.ScoreManager.ChangeScoreByTagByAmount(ScoreTags.PremiumCurrency, starIncrease);

            if (DBGameLogic.Instance.ScoreManager.TryGetScoreByTag(ScoreTags.PremiumCurrency, ref currencySaveData.StarsAmount))
            {
                InvokeEvent(DBEventNames.OnPremCurrencySet, currencySaveData.StarsAmount);
                SaveCurrency();
            }
        }

        public void SaveCurrency() => DBManager.Instance.SaveManager.Save(currencySaveData);

        public void LoadCurrency() =>
            DBManager.Instance.SaveManager.Load((CurrencySaveData data) =>
            currencySaveData = data ?? new CurrencySaveData(initialCoinsAmount, initialStarsAmount));
       
        private void UpdateCoinsAmountOnAction(object obj) => CoinsEarn(initialCoinsAmount);

        private void UpdateStarsAmountOnAction(object obj) => UpdateStarsCurrency(initialStarsAmount);

        private void RegisterEvents()
        {
            AddListener(DBEventNames.CurrencyUpdateUI, UpdateCoinsAmountOnAction);
            AddListener(DBEventNames.PremCurrencyUpdateUI, UpdateStarsAmountOnAction);
            AddListener(DBEventNames.AddCurrencyUpdate, CoinsEarn);
        }

        private void UnregisterEvents()
        {
            RemoveListener(DBEventNames.CurrencyUpdateUI, UpdateCoinsAmountOnAction);
            RemoveListener(DBEventNames.PremCurrencyUpdateUI, UpdateStarsAmountOnAction);
            RemoveListener(DBEventNames.AddCurrencyUpdate, CoinsEarn);
        }

        [Serializable]
        public class CurrencySaveData : IDBSaveData
        {
            public int CoinsAmount;
            public int StarsAmount;

            public CurrencySaveData(int coinsAmount, int starsAmount)
            {
                CoinsAmount = coinsAmount;
                StarsAmount = starsAmount;
            }
        }
    }
}
