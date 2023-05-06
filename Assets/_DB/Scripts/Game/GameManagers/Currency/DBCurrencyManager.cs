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

        #region Coins

        public void EarnCoins(object foodProfit)
        {
            int profit = (int)foodProfit;
            GameLogic.ScoreManager.ChangeScoreByTagByAmount(ScoreTags.GameCurrency, profit);

            if (GameLogic.ScoreManager.TryGetScoreByTag(ScoreTags.GameCurrency, ref currencySaveData.CoinsAmount))
            {
                InvokeEvent(DBEventNames.OnCurrencySet, currencySaveData.CoinsAmount);
                SaveCurrency();
            }
        }

        private void UpdateCoinsOnAction(object obj) => EarnCoins(initialCoinsAmount);

        #endregion

        #region Stars

        private void EarnStars(object stars)
        {
            int starIncrease = (int)stars;
            GameLogic.ScoreManager.ChangeScoreByTagByAmount(ScoreTags.PremiumCurrency, starIncrease);

            if (GameLogic.ScoreManager.TryGetScoreByTag(ScoreTags.PremiumCurrency, ref currencySaveData.StarsAmount))
            {
                InvokeEvent(DBEventNames.OnPremCurrencySet, currencySaveData.StarsAmount);
                SaveCurrency();
            }
        }

        private void UpdateStarsOnAction(object obj) => EarnStars(initialStarsAmount);

        #endregion

        #region Currency Data

        public void SaveCurrency() => Manager.SaveManager.Save(currencySaveData);

        public void LoadCurrency()
        {
            DBManager.Instance.SaveManager.Load((CurrencySaveData data) =>
            currencySaveData = data ?? new CurrencySaveData(initialCoinsAmount, initialStarsAmount));
        }

        #endregion

        #region Events

        private void RegisterEvents()
        {
            AddListener(DBEventNames.CurrencyUpdateUI, UpdateCoinsOnAction);
            AddListener(DBEventNames.PremCurrencyUpdateUI, UpdateStarsOnAction);
            AddListener(DBEventNames.AddCurrencyUpdate, EarnCoins);
            AddListener(DBEventNames.AddStarsUpdate, EarnStars);
        }

        private void UnregisterEvents()
        {
            RemoveListener(DBEventNames.CurrencyUpdateUI, UpdateCoinsOnAction);
            RemoveListener(DBEventNames.PremCurrencyUpdateUI, UpdateStarsOnAction);
            RemoveListener(DBEventNames.AddCurrencyUpdate, EarnCoins);
            RemoveListener(DBEventNames.AddStarsUpdate, EarnStars);
        }

        #endregion

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
