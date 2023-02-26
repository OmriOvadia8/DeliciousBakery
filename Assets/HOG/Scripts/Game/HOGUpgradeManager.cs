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

        public HOGUpgradeManager()
        {
              HOGManager.Instance.SaveManager.Load(delegate(HOGPlayerUpgradeInventoryData data)
            {
                PlayerUpgradeInventoryData = data ?? new HOGPlayerUpgradeInventoryData
                {
                    Upgradeables = new List<HOGUpgradeableData>(){new HOGUpgradeableData
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

                if (HOGGameLogic.Instance.ScoreManager.TryUseScore(currencyType, upgradeCost))
                {
                    upgradeable.CurrentLevel++;
                    HOGManager.Instance.EventsManager.InvokeEvent(HOGEventNames.OnCurrencySet, upgradeCost);
                    HOGManager.Instance.EventsManager.InvokeEvent(HOGEventNames.OnUpgraded, typeID);
                    HOGManager.Instance.SaveManager.Save(PlayerUpgradeInventoryData);
                }
            else
            {
                Debug.LogError($"UpgradeItemByID {typeID} tried upgrade and there is no enough");
            }
        }
        }

        public HOGUpgradeableData GetUpgradeableByID(UpgradeablesTypeID typeID, int index) // OVERLOAD Original  GetUpgradeableByID method
        {
            var upgradeable = PlayerUpgradeInventoryData.Upgradeables.FirstOrDefault(x => x.upgradableTypeID == typeID && x.foodID == index);
            return upgradeable;
        }

        public void UpgradeItemByID(UpgradeablesTypeID typeID)
        {
            var upgradeable = GetUpgradeableByID(typeID);

            if (upgradeable != null)
            {
                //TODO:Config + Reduce Score
                //var upgradeableConfig = GetHogUpgradeableConfigByID(typeID);
                //HOGUpgradeableLevelData levelData = upgradeableConfig.UpgradableLevelData[upgradeable.CurrentLevel + 1];
                //int amountToReduce = levelData.CoinsNeeded;
                //ScoreTags coinsType = levelData.CurrencyTag;

                //if (HOGGameLogic.Instance.ScoreManager.TryUseScore(coinsType, amountToReduce))
                {
                    upgradeable.CurrentLevel++;
                    HOGManager.Instance.EventsManager.InvokeEvent(HOGEventNames.OnUpgraded, typeID);
                }
                //else
                //{
                //    Debug.LogError($"UpgradeItemByID {typeID.ToString()} tried upgrade and there is no enough");
                //}
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
        public int foodID;
    }

    //Per Level in Item config
    [Serializable]
    public struct HOGUpgradeableLevelData
    {
        public int Level;
        public int CoinsNeeded;
        public ScoreTags CurrencyTag;
        public float cookingTime;
        public int costIncrease;
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
    public class HOGPlayerUpgradeInventoryData : IHOGSaveData
    {
        public List<HOGUpgradeableData> Upgradeables;
    }

    [Serializable]
    public enum UpgradeablesTypeID
    {
        Food = 0,
        Baker = 1,
        Equipment = 2
    }
}