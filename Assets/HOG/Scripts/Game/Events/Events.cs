using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;

public class Events : HOGMonoBehaviour
{
    private void Start()
    {
        AddListener("CookFood", CookFood);
    }

    private void OnDestroy()
    {
        Manager.EventsManager.RemoveListener("CookFood", CookFood);
    }

    public void CookFood(object obj)
    {
        // Code to start cooking the food
    }

    public void StartCooking()
    {
        Manager.EventsManager.InvokeEvent("CookFood", null);
    }
}

