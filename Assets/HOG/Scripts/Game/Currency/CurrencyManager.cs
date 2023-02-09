using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;
using TMPro;

namespace Game
{
    public abstract class CurrencyManager : HOGMonoBehaviour
    {
        public static CurrencyManager Instance;
        public TMP_Text moneyText;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            AddListener(HOGEventNames.UpdateCurrency, UpdateCurrencyDisplay);
        }

        private void UpdateCurrencyDisplay(object obj)
        {
            moneyText.text = obj.ToString();
        }

        public void IncreaseCurrency(int amount)
        {
            PlayerData.Instance.currency += amount;
        }

        public void DecreaseCurrency(int amount)
        {
            PlayerData.Instance.currency -= amount;
        }

        public int GetCurrency()
        {
            return PlayerData.Instance.currency;
        }
    }
}