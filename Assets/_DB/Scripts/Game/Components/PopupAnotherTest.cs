using DB_Core;

namespace DB_Game
{
    public class PopupAnotherTest : DBLogicMonoBehaviour
    {
       public void OnUpgradeButtonClicked()
        {
            DBPopupData upgradePopupData = DBPopupData.StorePopupData;
            Manager.PopupManager.AddPopupToQueue(upgradePopupData);
        }

        public void CloseWindow()
        {
            Manager.PopupManager.ClosePopup();
        }
    }
}