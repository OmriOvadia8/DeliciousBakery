using System.Collections.Generic;

namespace DB_Core
{
    public class DBPopupComponentBase : DBMonoBehaviour
    {
        protected DBPopupData popupData;

        public virtual void Init(DBPopupData popupData)
        {
            this.popupData = popupData;
            OnOpenPopup();
        }

        protected virtual void OnOpenPopup()
        {
            var data = new Dictionary<DBDataKeys, object>();
            data.Add(DBDataKeys.popup_type, popupData.PopupType.ToString());
            Manager.AnalyticsManager.ReportEvent(DBEventType.popup_open, data);

            popupData.OnPopupOpen?.Invoke();
        }

        public virtual void ClosePopup()
        {
            if (popupData != null)
            {
                OnClosePopup();
            }
        }


        protected virtual void OnClosePopup()
        {
            if (popupData == null)
            {
                return;
            }

            var data = new Dictionary<DBDataKeys, object>();
            data.Add(DBDataKeys.popup_type, popupData.PopupType.ToString());
            Manager.AnalyticsManager.ReportEvent(DBEventType.popup_close, data);

            popupData.OnPopupClose?.Invoke(this);
        }
    }
}