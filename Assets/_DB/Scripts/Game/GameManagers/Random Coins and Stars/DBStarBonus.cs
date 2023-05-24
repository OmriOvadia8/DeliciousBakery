using System.Collections;
using UnityEngine;
using DB_Core;
using DG.Tweening;

namespace DB_Game
{
    public class DBStarBonus : DBLogicMonoBehaviour
    {
        private bool collected;
        private int minBonus = 3;
        private int maxBonus = 10;
        [SerializeField] RectTransform statsIcon; 
        [SerializeField] Transform starTrans; 

        private void OnEnable()
        {
            collected = false;
            StartCoroutine(SelfDestructAfterDelay());
        }

        private IEnumerator SelfDestructAfterDelay()
        {
            yield return new WaitForSeconds(10);
            if (!collected)
            {
                this.gameObject.SetActive(false);
            }
        }

        public void GetRandomStarBonus()
        {
            collected = true;
            double bonus = Random.Range(minBonus, maxBonus + 1);
            InvokeBonusEvents(bonus);

            starTrans.DOMove(statsIcon.position, 1.5f).OnComplete(() => {
                this.gameObject.SetActive(false);
                collected = false;
            });
        }

        private void InvokeBonusEvents(double bonus)
        {
            InvokeEvent(DBEventNames.AddStarsUpdate, bonus);
            InvokeEvent(DBEventNames.PremCurrencyUpdateUI, null);
            InvokeEvent(DBEventNames.CheckBuySkinButtonUI, null);
            InvokeEvent(DBEventNames.CheckBuyTimeWrapButtonsUI, null);
            InvokeEvent(DBEventNames.PlaySound, SoundEffectType.UpgradeButtonClick);
        }
    }
}
