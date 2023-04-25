using System;
using System.Collections.Generic;
using System.Linq;
using DB_Core;
using UnityEngine;

namespace DB_Game
{
    public class DBUpgradeManager
    {
        public DBPlayerUpgradeInventoryData PlayerUpgradeInventoryData; //Player Saved Data
        public DBPlayerIdleFoodInventoryData PlayerIdleFoodInventoryData; // Player Saved Idle Food Data
        public DBUpgradeManagerConfig UpgradeConfig; //From cloud

        public DBUpgradeManager()
        {
            DBManager.Instance.SaveManager.Load(delegate (DBPlayerUpgradeInventoryData data)
          {
              PlayerUpgradeInventoryData = data ?? new DBPlayerUpgradeInventoryData
              {
                  Upgradeables = new List<DBUpgradeableData>(){new DBUpgradeableData
                        {
                            upgradableTypeID = UpgradeablesTypeID.Food,
                            CurrentLevel = 1,
                            foodID = 0
                        }
                  }
              };
          });
        }

        public void UpgradeItemByID(UpgradeablesTypeID typeID, int index, ScoreTags currencyType, int upgradeCost) // OVERLOAD orginal UpgradeItemByID method
        {
            var upgradeable = GetUpgradeableByID(typeID, index);

            if (upgradeable != null)
            {
                if (DBGameLogic.Instance.ScoreManager.TryUseScore(currencyType, upgradeCost))
                {
                    upgradeable.CurrentLevel++;
                    DBManager.Instance.EventsManager.InvokeEvent(DBEventNames.OnCurrencySet, upgradeCost);
                    DBManager.Instance.EventsManager.InvokeEvent(DBEventNames.OnUpgradeTextUpdate, typeID);
                    DBManager.Instance.SaveManager.Save(PlayerUpgradeInventoryData);

                    DBManager.Instance.AnalyticsManager.ReportEvent(DBEventType.upgrade_item, new Dictionary<DBDataKeys, object>()
                    {
                        {DBDataKeys.type_id, typeID.ToString()},
                        {DBDataKeys.upgrade_level, upgradeable.CurrentLevel}
                    });

                }
                else
                {
                    DBManager.Instance.AnalyticsManager.ReportEvent(DBEventType.try_upgrade_out_of, new Dictionary<DBDataKeys, object>()
                    {
                        {DBDataKeys.type_id, typeID.ToString()},
                        {DBDataKeys.upgrade_level, upgradeable.CurrentLevel}
                    });

                    Debug.LogError($"UpgradeItemByID {typeID} tried upgrade and there is no enough");
                }
            }
            else
            {
                DBManager.Instance.CrashManager.LogExceptionHandling($"UpgradeItemByID {typeID.ToString()} failed because upgradable was null");
            }

        }

        public DBUpgradeableData GetUpgradeableByID(UpgradeablesTypeID typeID, int index) // OVERLOAD Original  GetUpgradeableByID method
        {
            var upgradeable = PlayerUpgradeInventoryData.Upgradeables.FirstOrDefault(x => x.upgradableTypeID == typeID && x.foodID == index);
            return upgradeable;
        }

        public void UpgradeItemByID(UpgradeablesTypeID typeID)
        {
            var upgradeable = GetUpgradeableByID(typeID);

            if (upgradeable != null)
            {
                    upgradeable.CurrentLevel++;
                    DBManager.Instance.EventsManager.InvokeEvent(DBEventNames.OnUpgradeTextUpdate, typeID);
            }
        }

        public DBUpgradeableConfig GetDBUpgradeableConfigByID(UpgradeablesTypeID typeID)
        {
            DBUpgradeableConfig upgradeableConfig = UpgradeConfig.UpgradeableConfigs.FirstOrDefault(upgradable => upgradable.upgradableTypeID == typeID);
            return upgradeableConfig;
        }

        public DBUpgradeableData GetUpgradeableByID(UpgradeablesTypeID typeID)
        {
            var upgradeable = PlayerUpgradeInventoryData.Upgradeables.FirstOrDefault(x => x.upgradableTypeID == typeID);
            return upgradeable;
        }
    }

    //Per Player Owned Item
    [Serializable]
    public class DBUpgradeableData
    {
        public UpgradeablesTypeID upgradableTypeID;
        public int CurrentLevel;
        public int foodID;
    }

    //Per Level in Item config
    [Serializable]
    public struct DBUpgradeableLevelData
    {
        public int Level;
        public int CoinsNeeded;
        public ScoreTags CurrencyTag;
        public float cookingTime;
        public int costIncrease;
    }

    //Per Item Config
    [Serializable]
    public class DBUpgradeableConfig
    {
        public UpgradeablesTypeID upgradableTypeID;
        public List<DBUpgradeableLevelData> UpgradableLevelData;
    }

    //All config for upgradeable
    [Serializable]
    public class DBUpgradeManagerConfig
    {
        public List<DBUpgradeableConfig> UpgradeableConfigs;
    }

    //All player saved data
    [Serializable]
    public class DBPlayerUpgradeInventoryData : IDBSaveData
    {
        public List<DBUpgradeableData> Upgradeables;
    }

    [Serializable]
    public class DBPlayerIdleFoodInventoryData : IDBSaveData
    {
        public List<DBUpgradeableData> IdleFood;
    }

    [Serializable]
    public enum UpgradeablesTypeID
    {
        Food = 0,
        Baker = 1,
    }
}