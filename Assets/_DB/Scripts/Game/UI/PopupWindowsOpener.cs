using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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