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
        public int startingCurrency = 0;
        private void OnEnable()
        {
            PlayerCurrency();
            
        }
        private void Start()
        {
            UpdateCurrency(20);
        }
        public void PlayerCurrency()
        {
            Score.SetScoreByTag(ScoreTags.GameCurrency, startingCurrency);
        }
        public void UpdateCurrency(int foodProfit)
        {
            HOGGameLogic.Instance.ScoreManager.ChangeScoreByTagByAmount(ScoreTags.GameCurrency, foodProfit);
        }


    }
}