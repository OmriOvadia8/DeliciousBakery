using UnityEngine;
using Core;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


namespace Game
{
    public class HOGMoneyHolder
    {
        public HOGScoreManager Score => HOGGameLogic.Instance.ScoreManager;
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