using UnityEngine;
using DB_Core;
using TMPro;

namespace DB_Game
{
    public class WelcomeBackPopup : DBPopupComponentBase
    {
        private DBPauseCurrencyManager pauseCurrencyManager;

        [SerializeField]
        private Animator anim;

        [SerializeField]
        private TMP_Text messageText;

        private static readonly int Open = Animator.StringToHash("Open");

        public override void Init(DBPopupData popupData)
        {
            anim.SetTrigger(Open);
            anim.WaitForAnimationComplete(this, () => base.Init(popupData));
            pauseCurrencyManager = FindObjectOfType<DBPauseCurrencyManager>();
        }

        protected override void OnOpenPopup()
        {
            if(pauseCurrencyManager.PassedTimeFoodRewardCalc(Manager.TimerManager.GetLastOfflineTimeSeconds()) > 0)
            {
                messageText.SetText(popupData.MessageContent);
            }

            else
            {
                messageText.SetText(popupData.MessageNoProfitContent);
            }
            
            base.OnOpenPopup();
        }

        protected override void OnClosePopup()
        {
            base.OnClosePopup();
            InvokeEvent(DBEventNames.PlaySound, SoundEffectType.ButtonClick);
        }
    }
}