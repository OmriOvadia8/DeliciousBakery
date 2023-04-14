using UnityEngine;
using DB_Core;
using TMPro;
using UnityEngine.UI;

namespace DB_Game
{
    public class WelcomeBackPopup : DBPopupComponentBase
    {
        [SerializeField]
        private Animator anim;

        [SerializeField]
        private TMP_Text messageText;

        //[SerializeField]
        //private TMP_Text buttonText;

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
            if(DBPauseCurrencyManager.PassedTimeFoodRewardCalc(Manager.TimerManager.GetLastOfflineTimeSeconds()) > 0)
            {
                messageText.SetText(popupData.MessageContent);
               // buttonText.SetText(popupData.ButtonContent);
            }

            else
            {
                messageText.SetText(popupData.MessageNoProfitContent);
              //  buttonText.SetText(popupData.ButtonNoProfitContent);
            }
            
            base.OnOpenPopup();
        }
    }
}