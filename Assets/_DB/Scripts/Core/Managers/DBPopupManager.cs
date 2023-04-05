using System;
using System.Collections.Generic;
using System.Linq;

namespace DB_Core
{
    public class DBPopupManager
    {
        public List<DBPopupData> PopupDatas = new();

        public void AddPopupToQueue(DBPopupData popupData)
        {
            PopupDatas.Add(popupData);
            TryShowNextPopup();
        }

        public void TryShowNextPopup()
        {
            if (PopupDatas.Count <= 0)
            {
                return;
            }

            SortPopups();
            OpenPopup(PopupDatas[0]);
        }

        public void SortPopups()
        {
            PopupDatas = PopupDatas.OrderBy(x => x.Priority).ToList();
        }

        public void OpenPopup(DBPopupData dbPopupData)
        {
            //Invoke popup event open
            dbPopupData.OnPopupClose += OnClosePopup;
            PopupDatas.Remove(dbPopupData);
        }

        public void OnClosePopup()
        {
            //Invoke popup event close
            TryShowNextPopup();
        }
    }

    public class DBPopupData
    {
        public int Priority;
        public PopupTypes PopupType;

        public Action OnPopupOpen;
        public Action OnPopupClose;

        public object GenericData;
    }

    public enum PopupTypes
    {
        OfflineReward
    }
}