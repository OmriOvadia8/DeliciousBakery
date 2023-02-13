using UnityEngine;
using Core;
using System;


namespace Game
{
    public class HOGMoneyHolder
    {
        private HOGScoreManager Score => HOGGameLogic.Instance.ScoreManager;
        public int startingCurrency = 0;

        public void PlayerCurrency()
        {
            Score.SetScoreByTag(ScoreTags.GameCurrency, startingCurrency);
        }

        public void UpdateCurrency(int foodProfit)
        {
            Score.ChangeScoreByTagByAmount(ScoreTags.GameCurrency, foodProfit);
        }


    }
}