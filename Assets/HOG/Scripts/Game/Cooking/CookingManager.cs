using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Core;

namespace Game
{
    public class CookingManager : HOGMonoBehaviour
    {
        public FoodData[] foods;
        private bool[] isFoodOnCooldown;

        public HOGMoneyHolder Currency => HOGGameLogic.Instance.PlayerMoney;

        void Start()
        {

            int foodCount = 5;
            foods = new FoodData[foodCount];
            isFoodOnCooldown = new bool[foodCount];

            for (int i = 0; i < foodCount; i++)
            {
                switch (i)
                {
                    case 0:
                        foods[i] = new FoodData("Burger", 1, 5, 10, 100);
                        break;
                    case 1:
                        foods[i] = new FoodData("Bread", 1, 1, 5, 50);
                        break;
                    case 2:
                        foods[i] = new FoodData("Candy", 1, 1, 3, 20);
                        break;
                    case 3:
                        foods[i] = new FoodData("Pizza", 1, 3, 15, 200);
                        break;
                    case 4:
                        foods[i] = new FoodData("Ice Cream", 1, 2, 8, 80);
                        break;
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
            int profit = foods[foodIndex].profit * foods[foodIndex].level;

            StartCoroutine(StartCooking(cookingTime, profit, foodIndex));

        }

        private IEnumerator StartCooking(float cookingTime, int profit, int foodIndex)
        {
            isFoodOnCooldown[foodIndex] = true;

            yield return new WaitForSeconds(cookingTime);

            HOGGameLogic.Instance.PlayerMoney.UpdateCurrency(profit);

            Debug.Log(foodIndex); 
            
            isFoodOnCooldown[foodIndex] = false;
        }
    }
}




