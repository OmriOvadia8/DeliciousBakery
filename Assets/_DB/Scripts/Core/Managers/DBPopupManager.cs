using System.Collections.Generic;
using System;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Linq;
using System.Security.Claims;

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
    }

    public class DBPopupData
    {
        public int Priority;
        public PopupTypes PopupType;

        public Action OnPopupOpen;
        public Action<DBPopupComponentBase> OnPopupClose;

        public object GenericData;

        public static DBPopupData WelcomeBackMessage = new()
        {
            Priority = 0,
            PopupType = PopupTypes.WelcomeBackMessage,
            GenericData = "Claim 2X"
        };

        public static DBPopupData UpgradePopupData = new()
        {
            Priority = 0,
            PopupType = PopupTypes.UpgradePopupMenu
        };

        public static DBPopupData StorePopupData = new()
        {
            Priority = 0,
            PopupType = PopupTypes.Store
        };
    }

    public enum PopupTypes
    {
        WelcomeBackMessage,
        Store,
        UpgradePopupMenu,
        DoubleProfit
    }
}