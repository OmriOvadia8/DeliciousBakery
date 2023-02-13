using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using UnityEngine;

namespace Game
{
    public class HOGUpgradeManager
    {
        public HOGPlayerUpgradeInventoryData PlayerUpgradeInventoryData; //Player Saved Data
        public HOGUpgradeManagerConfig UpgradeConfig; //From cloud

        public void UpgradeItemByID(UpgradeablesTypeID typeID)
        {
            var upgradeable = GetUpgradeableByID(typeID);

            if (upgradeable != null)
            {
                var upgradeableConfig = GetHogUpgradeableConfigByID(typeID);
                HOGUpgradeableLevelData levelData = upgradeableConfig.UpgradableLevelData[upgradeable.CurrentLevel + 1];
                int amountToReduce = levelData.CoinsNeeded;
                ScoreTags coinsType = levelData.CurrencyTag;

                if (HOGGameLogic.Instance.ScoreManager.TryUseScore(coinsType, amountToReduce))
                {
                    upgradeable.CurrentLevel++;
                    HOGManager.Instance.EventsManager.InvokeEvent(HOGEventNames.OnUpgraded, typeID);
                }
                else
                {
                    Debug.LogError($"UpgradeItemByID {typeID.ToString()} tried upgrade and there is no enough");
                }
            }
        }

        public HOGUpgradeableConfig GetHogUpgradeableConfigByID(UpgradeablesTypeID typeID)
        {
            HOGUpgradeableConfig upgradeableConfig = UpgradeConfig.UpgradeableConfigs.FirstOrDefault(upgradable => upgradable.upgradableTypeID == typeID);
            return upgradeableConfig;
        }

        public HOGUpgradeableData GetUpgradeableByID(UpgradeablesTypeID typeID)
        {
            var upgradeable = PlayerUpgradeInventoryData.Upgradeables.FirstOrDefault(x => x.upgradableTypeID == typeID);
            return upgradeable;
        }
    }


    //Per Player Owned Item
    [Serializable]
    public class HOGUpgradeableData
    {
        public UpgradeablesTypeID upgradableTypeID;
        public int CurrentLevel;
    }

    //Per Level in Item config
    [Serializable]
    public struct HOGUpgradeableLevelData
    {
        public int Level;
        public int CoinsNeeded;
        public ScoreTags CurrencyTag;
        public string ArtItem;
        public int Power;
    }

    //Per Item Config
    [Serializable]
    public class HOGUpgradeableConfig
    {
        public UpgradeablesTypeID upgradableTypeID;
        public List<HOGUpgradeableLevelData> UpgradableLevelData;
    }

    //All config for upgradeable
    [Serializable]
    public class HOGUpgradeManagerConfig
    {
        public List<HOGUpgradeableConfig> UpgradeableConfigs;
    }

    //All player saved data
    [Serializable]
    public class HOGPlayerUpgradeInventoryData
    {
        public List<HOGUpgradeableData> Upgradeables;
    }

    [Serializable]
    public enum UpgradeablesTypeID
    {
        Upgradable1 = 0,
        Upgradeable2 = 1
    }
}