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
        public string ButtonContent;

        public string MessageNoProfitContent;
        public string ButtonNoProfitContent;

        public static DBPopupData WelcomeBackMessage = new()
        {
            Priority = 0,
            PopupType = PopupTypes.WelcomeBackMessage,

            MessageContent = "Welcome Back!\r\nYour bakers worked tirelessly while \r\nyou were away. \r\nHere's a reward for their hard work!",
           // ButtonContent = "Claim 2X",

            MessageNoProfitContent = "Welcome Back! \r\nNo profits earned while away.\r\nHire bakers to make money \r\nwhile offline!",
          //  ButtonNoProfitContent = "Got it!"
        };

        public static DBPopupData UpgradePopupData = new()
        {
            Priority = 0,
            PopupType = PopupTypes.UpgradePopupMenu
        };

        public static DBPopupData StorePopupData = new()
        {
            Priority = 10,
            PopupType = PopupTypes.Store
        };

        public static DBPopupData FirstLoginMessage = new()
        {
            Priority = 0,
            PopupType = PopupTypes.NoProfitWelcome
        };

    }

    public enum PopupTypes
    {
        WelcomeBackMessage,
        Store,
        UpgradePopupMenu,
        NoProfitWelcome
    }
}