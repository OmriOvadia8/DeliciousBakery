using UnityEngine;
using DB_Core;

namespace DB_Game
{
    public class FirstLoginPopupWindow : DBPopupComponentBase
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
            base.OnClosePopup();
            InvokeEvent(DBEventNames.PlaySound, SoundEffectType.ButtonClick);
        }
    }
}