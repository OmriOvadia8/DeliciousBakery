using Core;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace Game
{
    public class UIManager : HOGLogicMonoBehaviour
    {
        [SerializeField] TMP_Text moneyText;
        [SerializeField] private HOGMoneyHolder playerMoney;

        private void OnEnable()
        {
            AddListener(HOGEventNames.OnCurrencySet, OnMoneyUpdate);
        }

        private void OnDisable()
        {
            RemoveListener(HOGEventNames.OnCurrencySet, OnMoneyUpdate);
        }

        private void OnMoneyUpdate(object obj)
        {
            if (GameLogic.ScoreManager.TryGetScoreByTag(ScoreTags.GameCurrency, ref playerMoney.startingCurrency))
            {
                moneyText.text = playerMoney.startingCurrency.ToString();
            }
        }

    }
}