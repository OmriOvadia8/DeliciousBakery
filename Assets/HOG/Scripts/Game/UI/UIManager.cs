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
            int currency = 0;
            if (GameLogic.ScoreManager.TryGetScoreByTag(ScoreTags.GameCurrency, ref currency))
            {
                moneyText.text = currency.ToString();
            }
        }
    }
}