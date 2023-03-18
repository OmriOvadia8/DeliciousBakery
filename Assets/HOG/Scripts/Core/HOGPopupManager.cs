using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class HOGPopupManager
    {
        public List<HOGPopupData> PopupDatas = new();

        public void AddPopupToQueue(HOGPopupData popupData)
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

        public void OpenPopup(HOGPopupData hogPopupData)
        {
            //Invoke popup event open
            hogPopupData.OnPopupClose += OnClosePopup;
            PopupDatas.Remove(hogPopupData);
        }

        public void OnClosePopup()
        {
            //Invoke popup event close
            TryShowNextPopup();
        }
    }

    public class HOGPopupData
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