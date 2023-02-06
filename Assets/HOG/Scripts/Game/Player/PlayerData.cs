using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    protected CurrencyManager Money => MoneyManager.Instance;
    protected CurrencyManager Stars => StarsManager.Instance;

    public int currency;
    

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
