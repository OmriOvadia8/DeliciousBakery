using DB_Core;
using UnityEngine;

namespace DB_Game
{
    public class AdsStatusPopup : DBPopupComponentBase
    {
        [SerializeField]
        private Animator anim;

        private static readonly int Open = Animator.StringToHash("Open");

        public override void Init(DBPopupData popupData)
        {
            anim.SetTrigger(Open);
            anim.WaitForAnimationComplete(this, () => base.Init(popupData));
        }

        protected override void OnClosePopup()
        {
            InvokeEvent(DBEventNames.PlaySound, SoundEffectType.ButtonClick);
            base.OnClosePopup();
        }
    }
}