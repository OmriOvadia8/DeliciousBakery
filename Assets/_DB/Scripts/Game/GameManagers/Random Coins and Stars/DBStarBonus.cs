using System.Collections;
using UnityEngine;
using DB_Core;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

namespace DB_Game
{
    public class DBStarBonus : DBLogicMonoBehaviour
    {
        private bool collected;
        private int minBonus = 3;
        private int maxBonus = 10;
        [SerializeField] RectTransform statsIcon; 
        [SerializeField] Transform starTrans;
        [SerializeField] Button starButton;
        [SerializeField] TMP_Text starAmountText;

        private void OnEnable() => StartCoroutine(SelfDestructAfterDelay());

        private IEnumerator SelfDestructAfterDelay()
        {
            collected = false;
            yield return new WaitForSeconds(10);
            if (!collected)
            {
                this.gameObject.SetActive(false);
            }
        }

        public void GetRandomStarBonus()
        {
            collected = true;
            starButton.interactable = false;
            double bonus = Random.Range(minBonus, maxBonus + 1);
            InvokeBonusEvents(bonus);
            starTrans.DOMove(statsIcon.position, 2f).OnComplete(() =>
            {
                starButton.interactable = true;
                this.gameObject.SetActive(false);
            });
            ShowBonus(bonus);
            collected = false;
        }

        private void ShowBonus(double bonus)
        {
            PrepareTextForDisplay(starAmountText, bonus, starTrans.position);

            Sequence sequence = CreateBonusAnimationSequence(starAmountText);

            sequence.OnComplete(() => ResetAndHideText(starAmountText));
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
            InvokeEvent(DBEventNames.AddStarsUpdate, bonus);
            InvokeEvent(DBEventNames.PremCurrencyUpdateUI, null);
            InvokeEvent(DBEventNames.CheckBuySkinButtonUI, null);
            InvokeEvent(DBEventNames.CheckBuyTimeWrapButtonsUI, null);
            InvokeEvent(DBEventNames.PlaySound, SoundEffectType.UpgradeButtonClick);
        }
    }
}
