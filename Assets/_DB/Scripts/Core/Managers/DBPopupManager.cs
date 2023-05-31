using System.Collections.Generic;
using System;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Linq;

namespace DB_Core
{
    public class DBPopupManager
    {
        public List<DBPopupData> PopupsData = new();
        public Canvas popupsCanvas;

        private Dictionary<PopupTypes, DBPopupComponentBase> cachedPopups = new();

        public DBPopupManager()
        {
            CreateCanvas();
        }

        private void CreateCanvas()
        {
            DBManager.Instance.FactoryManager.CreateAsync("PopupCanvas", Vector3.zero, (Canvas canvas) =>
            {
                popupsCanvas = canvas;
                Object.DontDestroyOnLoad(popupsCanvas);
            });
        }

        public void AddPopupToQueue(DBPopupData popupData)
        {
            PopupsData.Add(popupData);
            TryShowNextPopup();
        }

        public void TryShowNextPopup()
        {
            if (PopupsData.Count <= 0)
            {
                return;
            }

            SortPopups();
            OpenPopup(PopupsData[0]);
        }

        public void SortPopups()
        {
            PopupsData = PopupsData.OrderBy(x => x.Priority).ToList();
        }

        public void OpenPopup(DBPopupData dbPopupData)
        {
            dbPopupData.OnPopupClose += OnClosePopup;
            PopupsData.Remove(dbPopupData);

            if (cachedPopups.ContainsKey(dbPopupData.PopupType))
            {
                var pop = cachedPopups[dbPopupData.PopupType];
                pop.gameObject.SetActive(true);
                pop.Init(dbPopupData);
            }
            else
            {
                DBManager.Instance.FactoryManager.CreateAsync(dbPopupData.PopupType.ToString(),
                    Vector3.zero, (DBPopupComponentBase popupComponent) =>
                    {
                        cachedPopups.Add(dbPopupData.PopupType, popupComponent);
                        popupComponent.transform.SetParent(popupsCanvas.transform, false);
                        popupComponent.Init(dbPopupData);
                    });
            }
        }

        private void OnClosePopup(DBPopupComponentBase dbPopupComponentBase)
        {
            dbPopupComponentBase.gameObject.SetActive(false);
            TryShowNextPopup();
        }

        public void ClosePopup()
        {
            if (cachedPopups.Count > 0)
            {
                var lastPopup = cachedPopups.Values.LastOrDefault(popup => popup.gameObject.activeSelf);
                if (lastPopup != null)
                {
                    lastPopup.ClosePopup();
                }
            }
        }
    }

    public class DBPopupData
    {
        public int Priority;
        public PopupTypes PopupType;
        public Action OnPopupOpen;
        public Action<DBPopupComponentBase> OnPopupClose;
        public string MessageContent;
        public string MessageNoProfitContent;

        public static DBPopupData WelcomeBackMessage = new()
        {
            Priority = 10,
            PopupType = PopupTypes.WelcomeBackMessage,
            MessageContent = "Welcome Back!\r\nYour bakers worked tirelessly while \r\nyou were away. \r\nHere's a reward for their hard work!",
            MessageNoProfitContent = "Welcome Back! \r\nNo profits earned while away.\r\nHire bakers to make money \r\nwhile offline!",
        };

        public static DBPopupData StorePopupData = new()
        {
            Priority = 1,
            PopupType = PopupTypes.Store
        };

        public static DBPopupData FirstLoginMessage = new()
        {
            Priority = 10,
            PopupType = PopupTypes.FirstLoginMessage
        };

        public static DBPopupData LoadingAd = new()
        {
            Priority = 1,
            PopupType = PopupTypes.LoadingAd
        };

        public static DBPopupData LoadingAdFailed = new()
        {
            Priority = 5,
            PopupType = PopupTypes.LoadingAdFailed
        };
    }

    public enum PopupTypes
    {
        WelcomeBackMessage,
        Store,
        UpgradePopupMenu,
        FirstLoginMessage,
        LoadingAd,
        LoadingAdFailed
    }
}