using UnityEngine;
using Core;
using System;

namespace Game
{
    public class HOGMoneyHolder : HOGLogicMonoBehaviour
    {
        public int startingCurrency = 0;
        CurrencySaveData currencySaveData = new CurrencySaveData(0);

        private void Awake()
        {
            PlayerCurrency();
            LoadCurrency();
        }


        public void PlayerCurrency()
        {
            GameLogic.ScoreManager.SetScoreByTag(ScoreTags.GameCurrency, startingCurrency);
        }

        public void UpdateCurrency(int foodProfit)
        {
            GameLogic.ScoreManager.ChangeScoreByTagByAmount(ScoreTags.GameCurrency, foodProfit);
            if (HOGGameLogic.Instance.ScoreManager.TryGetScoreByTag(ScoreTags.GameCurrency, ref currencySaveData.CurrencyAmount))
            {
                InvokeEvent(HOGEventNames.OnCurrencySet, currencySaveData.CurrencyAmount);
            }
            SaveCurrency();
        }

        public void SaveCurrency()
        {
            HOGManager.Instance.SaveManager.Save(currencySaveData);
            Debug.Log(currencySaveData.CurrencyAmount + " saved");
        }

        public void LoadCurrency()
        {
            HOGManager.Instance.SaveManager.Load<CurrencySaveData>(data =>
            {
                if (data != null)
                {
                    currencySaveData = data;
                    startingCurrency = data.CurrencyAmount;
                }
                else
                    PlayerCurrency();

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
