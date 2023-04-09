using UnityEngine;
using DB_Core;
using TMPro;

namespace DB_Game
{
    public class WelcomeBackPopup : DBPopupComponentBase
    {
        [SerializeField]
        private Animator anim;

        [SerializeField]
        private TMP_Text messageText;

        [SerializeField]
        private TMP_Text buttonText;

        private static readonly int Open = Animator.StringToHash("Open");

        public override void Init(DBPopupData popupData)
        {
            anim.SetTrigger(Open);
            anim.WaitForAnimationComplete(this, () => base.Init(popupData));
        }

        protected override void OnOpenPopup()
        {
            if(PauseCurrencyManager.PassedTimeFoodRewardCalc(Manager.TimerManager.GetLastOfflineTimeSeconds()) > 0)
            {
                messageText.SetText(popupData.MessageContent);
                buttonText.SetText(popupData.ButtonContent);
            }

            else
            {
                messageText.SetText(popupData.MessageNoProfitContent);
                buttonText.SetText(popupData.ButtonNoProfitContent);
            }
            
            base.OnOpenPopup();
        }
    }
}