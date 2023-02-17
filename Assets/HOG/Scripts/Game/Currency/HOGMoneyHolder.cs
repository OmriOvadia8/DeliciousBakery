using UnityEngine;
using Core;
using System;


namespace Game
{
    public class HOGMoneyHolder : HOGLogicMonoBehaviour
    {
        public int startingCurrency = 0;

        public void PlayerCurrency()
        {
            GameLogic.ScoreManager.SetScoreByTag(ScoreTags.GameCurrency, startingCurrency);
        }

        public void UpdateCurrency(int foodProfit)
        {
            GameLogic.ScoreManager.ChangeScoreByTagByAmount(ScoreTags.GameCurrency, foodProfit);
            InvokeEvent(HOGEventNames.OnCurrencySet, startingCurrency);
            Debug.Log(startingCurrency);
        }
    }
}