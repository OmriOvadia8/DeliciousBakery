using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Core;

public class CookingManager : HOGMonoBehaviour
{
    public FoodData[] foods;
    private bool[] isFoodOnCooldown;
    public Button[] levelUpButtons;

    

    void Start()
    {

        isFoodOnCooldown = new bool[foods.Length];
        for (int i = 0; i < foods.Length; i++)
        {
            int foodIndex = i;
            levelUpButtons[i].onClick.AddListener(() => LevelUpFood(foodIndex));
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < foods.Length; i++)
        {
            int foodIndex = i;
            levelUpButtons[i].onClick.RemoveListener(() => LevelUpFood(foodIndex));
        }
    }

    private void Update()
    {
        for (int i = 0; i < foods.Length; i++)
        {
            if (CurrencyManager.Instance.GetCurrency() >= foods[i].levelUpCost)
            {
                levelUpButtons[i].interactable = true;
            }
            else
            {
                levelUpButtons[i].interactable = false;
            }
        }
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
        CurrencyManager.Instance.moneyText.text = CurrencyManager.Instance.GetCurrency().ToString();

        isFoodOnCooldown[foodIndex] = false;
    }

    public void LevelUpFood(int foodIndex)
    {
        // Check if the player has enough currency to level up the food item
        if (CurrencyManager.Instance.GetCurrency() >= foods[foodIndex].levelUpCost)
        {
            PlayerData.Instance.currency -= foods[foodIndex].levelUpCost;
            foods[foodIndex].level++;
            Debug.Log(foods[foodIndex].level);

            InvokeEvent(CurrencyManager.UpdateCurrency, CurrencyManager.Instance.GetCurrency());
            
       
        }
    }

}
