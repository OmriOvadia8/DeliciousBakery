using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public float currency = 0;

    public PlayerData(float startingCurrency)
    {
        currency = startingCurrency;
    }
}
