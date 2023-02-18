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
        [SerializeField] TMP_Text[] foodLevelTextFields;
        [SerializeField] FoodManager foodManager;

        private void OnEnable()
        {
            AddListener(HOGEventNames.OnCurrencySet, OnMoneyUpdate);
            AddListener(HOGEventNames.OnUpgraded, OnUpgradeUpdate);
        }

        private void OnDisable()
        {
            RemoveListener(HOGEventNames.OnCurrencySet, OnMoneyUpdate);
            RemoveListener(HOGEventNames.OnUpgraded, OnUpgradeUpdate);
        }

        private void OnMoneyUpdate(object obj)
        {
            int currency = 0;
            if (GameLogic.ScoreManager.TryGetScoreByTag(ScoreTags.GameCurrency, ref currency))
            {
                moneyText.text = currency.ToString();
            }
        }

        private void OnUpgradeUpdate(object obj)
        {
   
        }


    }
}