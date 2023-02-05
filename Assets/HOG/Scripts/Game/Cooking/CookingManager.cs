using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingManager : MonoBehaviour
{
    public FoodData[] foods;

    public void CookFood(int foodIndex)
    {
        float cookingTime = foods[foodIndex].cookingTime;
        float profit = foods[foodIndex].profit;

        StartCoroutine(StartCooking(cookingTime, profit));
    }

    private IEnumerator StartCooking(float cookingTime, float profit)
    {
        yield return new WaitForSeconds(cookingTime);

        CurrencyManager.Instance.IncreaseCurrency(profit);
    }
}
