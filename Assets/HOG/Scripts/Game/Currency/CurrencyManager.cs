using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CurrencyManager : MonoBehaviour
{
    public PlayerData playerData;
    public static CurrencyManager Instance;

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


    public void IncreaseCurrency(int amount)
    {
        playerData.currency += amount;
    }

    public void DecreaseCurrency(int amount)
    {
        playerData.currency -= amount;
    }

    public int GetCurrency()
    {
        return playerData.currency;
    }
}
