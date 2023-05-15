//using DB_Core;
//using System;
//using System.Collections.Generic;
//using UnityEngine;

//namespace DB_Game
//{
//    [Serializable]
//    public class DBSkinData
//    {
//        public string DeviceName;
//        public string Color;
//        public int Cost;
//        public bool IsUnlocked;
//        public bool IsEquipped;

//        public DBSkinData(string deviceName, string color, int cost, bool isUnlocked, bool isEquipped)
//        {
//            DeviceName = deviceName;
//            Color = color;
//            Cost = cost;
//            IsUnlocked = isUnlocked;
//            IsEquipped = isEquipped;
//        }
//    }

//    public class SkinDatabase : IDBSaveData
//    {
//        public List<DBSkinData> Skins;
//    }
//}