//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Core;
//using UnityEngine.UI;

//public class UIChanges : HOGMonoBehaviour
//{
//    [SerializeField] private List<Button> buttons;
//    public FoodData[] foods;

//    private void Start()
//    {
        
//    }

//    private void OnEnable()
//    {
//        AddListener("CurrencyChanged", OnCurrencyChanged);
//    }

//    private void OnDisable()
//    {
//        RemoveListener("CurrencyChanged", OnCurrencyChanged);
//    }

//    private void OnCurrencyChanged(object obj)
//    {
//        if (CurrencyManager.Instance.GetCurrency() >= foods[obj].levelUpCost)
//        {
//            button.interactable = true;
//        }
//        else
//        {
//            button.interactable = false;
//        }
//    }


//}
