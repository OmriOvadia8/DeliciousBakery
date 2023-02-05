using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CookingManager : MonoBehaviour
{
    public FoodData[] foods;
    private bool[] isFoodOnCooldown;
    public TMP_Text moneyText;

    private void Start()
    {
        isFoodOnCooldown = new bool[foods.Length];
    }

    public void CookFood(int foodIndex)
    {
        if (isFoodOnCooldown[foodIndex])
        {
            return;
        }

        float cookingTime = foods[foodIndex].cookingTime;
        int profit = foods[foodIndex].profit;

        StartCoroutine(StartCooking(cookingTime, profit, foodIndex));
    }

    private IEnumerator StartCooking(float cookingTime, int profit, int foodIndex)
    {
        isFoodOnCooldown[foodIndex] = true;

        yield return new WaitForSeconds(cookingTime);

        CurrencyManager.Instance.IncreaseCurrency(profit);
        moneyText.text = CurrencyManager.Instance.GetCurrency().ToString();

        isFoodOnCooldown[foodIndex] = false;
    }
}
