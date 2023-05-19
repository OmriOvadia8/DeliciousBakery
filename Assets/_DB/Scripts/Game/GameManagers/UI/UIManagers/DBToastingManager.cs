using System;
using DB_Core;
using UnityEngine;

namespace DB_Game
{
    public class DBToastingManager : DBLogicMonoBehaviour
    {
        [SerializeField] private RectTransform moneyToastPosition;
        [SerializeField] private int moneyTextToastAmount = 20;
        private DBTweenMoneyComponent currentToast;

        private void Start() => MoneyToastPoolInitialization();

        public void DisplayMoneyToast(double moneyAmount, PoolNames poolName)
        {
            if (currentToast != null)
            {
                currentToast.HideImmediately();
            }

            var moneyToast = (DBTweenMoneyComponent)Manager.PoolManager.GetPoolable(poolName);
            Vector3 toastPosition = moneyToastPosition.position;
            moneyToast.transform.position = toastPosition;

            currentToast = moneyToast;

            switch (poolName)
            {
                case PoolNames.MoneyToast:
                    moneyToast.Init(moneyAmount);
                    break;

                case PoolNames.SpendMoneyToast:
                    moneyToast.SpendInit(moneyAmount);
                    break;

                default:
                    throw new ArgumentException("Invalid PoolName value");
            }
        }

        private void MoneyToastPoolInitialization()
        {
            Manager.PoolManager.InitPool(PoolNames.MoneyToast.ToString(), moneyTextToastAmount, moneyToastPosition);
            Manager.PoolManager.InitPool(PoolNames.SpendMoneyToast.ToString(), moneyTextToastAmount, moneyToastPosition);
        }
    }
}