using UnityEngine;
using Core;
using System;

namespace Game
{
    public class HOGMoneyHolder : HOGLogicMonoBehaviour
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
            if (HOGGameLogic.Instance.ScoreManager.TryGetScoreByTag(ScoreTags.GameCurrency, ref currencySaveData.CurrencyAmount))
            {
                InvokeEvent(HOGEventNames.OnCurrencySet, currencySaveData.CurrencyAmount);
                SaveCurrency();
            }
        }

        public void SaveCurrency() // saving the current player's currency
        {
            HOGManager.Instance.SaveManager.Save(currencySaveData);
            HOGDebug.Log(currencySaveData.CurrencyAmount + " saved");
        }

        public void LoadCurrency() // loading the latest player's currency and if there is no save then starts at 0
        {
            HOGManager.Instance.SaveManager.Load<CurrencySaveData>(delegate (CurrencySaveData data)
            {
                currencySaveData = data ?? new CurrencySaveData(0);
            });
        }

        [Serializable]
        public class CurrencySaveData : IHOGSaveData
        {
            public int CurrencyAmount;

            public CurrencySaveData(int currencyAmount)
            {
                CurrencyAmount = currencyAmount;
            }
        }
    }
}
