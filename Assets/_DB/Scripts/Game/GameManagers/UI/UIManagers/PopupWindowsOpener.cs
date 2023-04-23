using DB_Core;

namespace DB_Game
{
    public class PopupWindowsOpener : DBLogicMonoBehaviour
    {
        public void OpenStorePopup()
        {
            Manager.PopupManager.AddPopupToQueue(DBPopupData.StorePopupData);
        }

    }
}