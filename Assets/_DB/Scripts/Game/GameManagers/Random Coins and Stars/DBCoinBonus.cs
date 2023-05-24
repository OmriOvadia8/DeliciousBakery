using System.Collections;
using UnityEngine;
using DB_Core;
using DG.Tweening;

namespace DB_Game
{
    public class DBCoinBonus : DBLogicMonoBehaviour
    {
        private bool collected;
        [SerializeField] DBCurrencyManager currencyManager;
        [SerializeField] RectTransform statsIcon;
        [SerializeField] Transform coinTrans;
        private readonly float currencyPercentageForBonus = 0.1f;
        private int starterBonus = 100;

        private void Start() => StartCoroutine(SelfDestructAfterDelay());
  
        private IEnumerator SelfDestructAfterDelay()
        {
            collected = false;
            yield return new WaitForSeconds(10); 
            if (!collected)
            {
                this.gameObject.SetActive(false);
            }
        }

        public void GetRandomCoinBonus()
        {
            collected = true;
            double bonus = currencyPercentageForBonus * currencyManager.currencySaveData.CoinsAmount + starterBonus;
            InvokeBonusEvents(bonus);

            coinTrans.DOMove(statsIcon.position, 1.5f).OnComplete(() => {
                this.gameObject.SetActive(false);
            });

            collected = false;
        }

        private void InvokeBonusEvents(double bonus)
        {
            InvokeEvent(DBEventNames.AddCurrencyUpdate, bonus);
            InvokeEvent(DBEventNames.CurrencyUpdateUI, null);
            InvokeEvent(DBEventNames.BuyButtonsCheck, null);
            InvokeEvent(DBEventNames.PlaySound, SoundEffectType.UpgradeButtonClick);
        }
    }
}