using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance;
    protected CurrencyManager Money => MoneyManager.Instance;
    protected CurrencyManager Stars => StarsManager.Instance;

    public int currency;

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


    public PlayerData(int startingCurrency)
    {
        currency = startingCurrency;
    }

    public void BuyRecipe(int recipeCost)
    {
        Money.DecreaseCurrency(recipeCost);
    }

    public void FoodUpgrade(int foodUpgradeCost)
    {
        Money.DecreaseCurrency(foodUpgradeCost);
    }

    public void EquipUpgrade(int equipUpgradeCost)
    {
        Money.DecreaseCurrency(equipUpgradeCost);
    }
}
