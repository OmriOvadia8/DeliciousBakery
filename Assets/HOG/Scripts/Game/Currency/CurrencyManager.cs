using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
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


    public void IncreaseCurrency(float amount)
    {
        playerData.currency += amount;
    }

    public void DecreaseCurrency(float amount)
    {
        playerData.currency -= amount;
    }
}
