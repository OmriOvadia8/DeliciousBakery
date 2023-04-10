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
           // Manager.PopupManager.AddPopupToQueue(DBPopupData.StorePopupData);
            base.OnClosePopup();
        }

        protected override void OnOpenPopup()
        {
            base.OnOpenPopup();
        }
    }
}