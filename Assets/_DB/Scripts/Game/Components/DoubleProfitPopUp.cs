using UnityEngine;
using UnityEngine.UI;
using DB_Core;

namespace DB_Game
{
    public class DoubleProfitPopUp : DBPopupComponentBase
    {
        [SerializeField] private GameObject popupPrefab;

        public void Open2XProfitWindow()
        {
            // Instantiate the popup prefab
            GameObject popup = Instantiate(popupPrefab, transform);

            //// Add the popup data to the popup manager queue
            //DBPopupData popupData = new DBPopupData
            //{
            //    Priority = 1,
            //    PopupType = PopupTypes.DoubleProfit,
            //    OnPopupOpen = () => Debug.Log("Popup opened"),
            //    OnPopupClose = () => Debug.Log("Popup closed")
            //};
          //  DBManager.Instance.PopupManager.AddPopupToQueue(popupData);
        }
    }
}