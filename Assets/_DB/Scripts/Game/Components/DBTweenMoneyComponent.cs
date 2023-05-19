using DG.Tweening;
using DB_Core;
using TMPro;
using UnityEngine;

namespace DB_Game
{
    public class DBTweenMoneyComponent : DBPoolable
    {
        [SerializeField] TMP_Text moneyToastText;

        [SerializeField] private float tweenTime = 1f;
        [SerializeField] private Vector3 moveAmount = Vector3.up;
        [SerializeField] private float fadeTarget;

        [SerializeField] private float scaleStart;
        [SerializeField] private float scaleEnd = 1f;

        [SerializeField] private Ease easeTypeMove = Ease.Linear;
        [SerializeField] private AnimationCurve fadeEase;

        private Tween fadeTween;
        private Tween moveTween;
        private Tween scaleTween;

        public void Init(double amount)
        {
            moneyToastText.text = $"+{amount.ToReadableNumber()}";
            DoAnimation();
        }

        public void SpendInit(double amount)
        {
            moneyToastText.text = $"-{amount.ToReadableNumber()}";
            DoAnimation();
        }

        public void DoAnimation()
        {
            transform.DOKill();

            transform.localScale = scaleStart * Vector3.one;
            moneyToastText.color = new Color(moneyToastText.color.r, moneyToastText.color.g, moneyToastText.color.b, 1f);
   
            transform.DOScale(scaleEnd, tweenTime);
            moneyToastText.DOFade(fadeTarget, tweenTime).SetEase(fadeEase);
            transform.DOLocalMove(transform.localPosition + moveAmount, tweenTime).SetEase(easeTypeMove).OnComplete(() => ReturnToPool());
        }

        public void HideImmediately()
        {
            fadeTween?.Kill();
            moveTween?.Kill();
            scaleTween?.Kill();

            ReturnToPool();
        }

        public void ReturnToPool()
        {
            Manager.PoolManager.ReturnPoolable(this);
        }

        public override void OnReturnedToPool()
        {
            var tempColor = moneyToastText.color;
            tempColor.a = 1;
            moneyToastText.color = tempColor;
            base.OnReturnedToPool();
        }
    }
}