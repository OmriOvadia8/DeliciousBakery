using UnityEngine;
using Core;
using System;


namespace Game
{
    public class HOGMoneyHolder : HOGLogicMonoBehaviour
    {
        public int startingCurrency = 0;

        private void OnEnable()
        {
            PlayerCurrency();
        }

        public void PlayerCurrency()
        {
            GameLogic.ScoreManager.SetScoreByTag(ScoreTags.GameCurrency, startingCurrency);
        }

        public void UpdateCurrency(int foodProfit)
        {
            GameLogic.ScoreManager.ChangeScoreByTagByAmount(ScoreTags.GameCurrency, foodProfit);
            if (HOGGameLogic.Instance.ScoreManager.TryGetScoreByTag(ScoreTags.GameCurrency, ref startingCurrency))
            {
                InvokeEvent(HOGEventNames.OnCurrencySet, startingCurrency);
            }
        }
    }
}