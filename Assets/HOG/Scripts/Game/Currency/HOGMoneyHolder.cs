using UnityEngine;
using Core;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


namespace Game
{
    public class HOGMoneyHolder : HOGMonoBehaviour
    {
        public HOGScoreManager Score => HOGGameLogic.Instance.ScoreManager;
        public int startingCurrency;
        private void OnEnable()
        {
            PlayerCurrency();
            HOGGameLogic.Instance.ScoreManager.ChangeScoreByTagByAmount(ScoreTags.GameCurrency, 20);
        }

        public void PlayerCurrency()
        {
            InvokeEvent(HOGEventNames.OnScoreSet, (ScoreTags.GameCurrency, startingCurrency = 0));
        }
        public void UpdateCurrency(int foodProfit)
        {
            HOGGameLogic.Instance.ScoreManager.ChangeScoreByTagByAmount(ScoreTags.GameCurrency, foodProfit);
        }


    }
}