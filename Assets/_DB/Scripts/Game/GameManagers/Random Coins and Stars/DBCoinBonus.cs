using System.Collections;
using UnityEngine;
using DB_Core;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

namespace DB_Game
{
    public class DBCoinBonus : DBLogicMonoBehaviour
    {
        [SerializeField] DBCurrencyManager currencyManager;
        [SerializeField] RectTransform statsIcon;
        [SerializeField] Transform coinTrans;
        [SerializeField] Button coinButton;
        [SerializeField] TMP_Text moneyAmountText;

        private bool collected;
        private float currencyPercentageForBonus;
        private int starterBonus = 100;

        private void Start() => StartCoroutine(SelfDestructAfterDelay());

        private IEnumerator SelfDestructAfterDelay()
        {
            collected = false;
            yield return new WaitForSeconds(8);
            if (!collected)
            {
                this.gameObject.SetActive(false);
            }
        }

        public void GetRandomCoinBonus()
        {
            collected = true;
            coinButton.interactable = false;
            currencyPercentageForBonus = Random.Range(0.011f, 0.025f);
            double bonus = currencyPercentageForBonus * currencyManager.currencySaveData.CoinsAmount + starterBonus;
            InvokeBonusEvents(bonus);
            coinTrans.DOMove(statsIcon.position, 2f).OnComplete(() =>
            {
                coinButton.interactable = true;
                this.gameObject.SetActive(false);
            });
            ShowBonus(bonus);
            collected = false;
        }

        private void ShowBonus(double bonus)
        {
            PrepareTextForDisplay(moneyAmountText, bonus, coinTrans.position);
            Sequence sequence = CreateBonusAnimationSequence(moneyAmountText);
            sequence.OnComplete(() => ResetAndHideText(moneyAmountText));
        }

        private void PrepareTextForDisplay(TMP_Text textElement, double bonus, Vector3 position)
        {
            textElement.gameObject.transform.position = position;
            textElement.text = "+ " + bonus.ToReadableNumber();
            textElement.transform.localScale = Vector3.one;
            textElement.color = new Color(textElement.color.r, textElement.color.g, textElement.color.b, 1);
            textElement.gameObject.SetActive(true);
        }

        private Sequence CreateBonusAnimationSequence(TMP_Text textElement)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(textElement.DOFade(1, 0.4f));
            sequence.Append(textElement.transform.DOScale(1.2f, 0.4f).SetLoops(2, LoopType.Yoyo));
            sequence.Append(textElement.transform.DOLocalMoveY(50f, 0.6f));
            sequence.Append(textElement.DOFade(0, 0.1f));

            return sequence;
        }

        private void ResetAndHideText(TMP_Text textElement)
        {
            textElement.gameObject.SetActive(false);
            textElement.DOKill();
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