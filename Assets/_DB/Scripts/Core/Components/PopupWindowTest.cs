using TMPro;
using UnityEngine;

namespace DB_Core
{
    public class PopupWindowTest : DBPopupComponentBase
    {
        [SerializeField]
        private Animator anim;

        [SerializeField]
        private TMP_Text message;

        private static readonly int Open = Animator.StringToHash("Open");

        public override void Init(DBPopupData popupData)
        {
            anim.SetTrigger(Open);
            anim.WaitForAnimationComplete(this, () => base.Init(popupData));
        }

        protected override void OnOpenPopup()
        {
            message.SetText((string)popupData.GenericData);
            base.OnOpenPopup();
        }
    }
}