using DG.Tweening;
using Core;
using TMPro;
using UnityEngine;

namespace Game
{
    public class HOGTweenMoneyComponent : HOGPoolable
    {
        [SerializeField] TMP_Text moneyToastText;

        [SerializeField] private float tweenTime = 1f;
        [SerializeField] private Vector3 moveAmount = new Vector3(0f, 1f, 0f);
        [SerializeField] private float fadeTarget = 0f;

        [SerializeField] private float scaleStart = 0f;
        [SerializeField] private float scaleEnd = 1f;

        [SerializeField] private Ease easeTypeMove = Ease.OutCubic;
        [SerializeField] private AnimationCurve fadeEase;


        public void Init(int amount)
        {
            moneyToastText.text = $"+{amount:N0}"; ;
            DoAnimation();
        }

        public void DoAnimation()
        {
            transform.localScale = scaleStart * Vector3.one;

            moneyToastText.DOFade(fadeTarget, tweenTime).SetEase(fadeEase);
            transform.DOLocalMove(transform.localPosition + moveAmount, tweenTime).SetEase(easeTypeMove);
            transform.DOScale(scaleEnd * Vector3.one, tweenTime).OnComplete(() =>
            {
                Manager.PoolManager.ReturnPoolable(this);
            });
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