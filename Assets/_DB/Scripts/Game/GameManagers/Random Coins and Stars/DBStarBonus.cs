using System.Collections;
using UnityEngine;
using DB_Core;

namespace DB_Game
{
    public class DBStarBonus : DBLogicMonoBehaviour
    {
        private bool collected;
        private int minBonus = 5;
        private int maxBonus = 10;

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
            this.gameObject.SetActive(false);
            collected = false;
        }

        private void InvokeBonusEvents(double bonus)
        {
            InvokeEvent(DBEventNames.AddStarsUpdate, bonus);
            InvokeEvent(DBEventNames.PremCurrencyUpdateUI, null);
            InvokeEvent(DBEventNames.CheckBuySkinButtonUI, null);
        }
    }
}