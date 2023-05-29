using DB_Core;

namespace DB_Game
{
    public class PopupWindowsOpener : DBLogicMonoBehaviour
    {
        public void OpenStorePopup() => Manager.PopupManager.AddPopupToQueue(DBPopupData.StorePopupData);
  
        // I'll add more popups to open here if I create more
    }
}