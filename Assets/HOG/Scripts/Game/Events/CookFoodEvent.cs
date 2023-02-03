using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;

public class CookFoodEvent : MonoBehaviour
{
    public HOGEvent cookFood;

    private void Start()
    {
        cookFood = new HOGEvent
        {
            eventName = "CookFood",
            eventAction = CookFood
        };

        HOGManager.Instance.EventsManager.AddListener(cookFood);
    }

    private void OnDestroy()
    {
        HOGManager.Instance.EventsManager.RemoveListener(cookFood);
    }

    public void CookFood(object obj)
    {
        // Code to start cooking the food
    }

    public void StartCooking()
    {
        HOGManager.Instance.EventsManager.InvokeEvent("CookFood", null);
    }
}

