//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using DB_Core;

//namespace DB_Game
//{
//    public class DBSkinsManager : DBLogicMonoBehaviour
//    {
//        public SkinSelectionData skinSelectionData;

//        private void Start()
//        {
//            // Load the data from Firebase
//            Manager.SaveManager.Load<SkinSelectionData>(data =>
//            {
//                if (data != null)
//                {
//                    skinSelectionData = data;
//                    ApplySavedSkins();
//                }
//            });
//        }

//        private void ApplySavedSkins()
//        {
//            // For each device in the saved data, apply the saved skin selection
//            foreach (KeyValuePair<string, int> entry in skinSelectionData.skinSelections)
//            {
//                string deviceName = entry.Key;
//                int skinIndex = entry.Value;

//                // Find the device by its name
//                Device device = GameObject.Find(deviceName)?.GetComponent<Device>();
//                if (device != null)
//                {
//                    device.ChangeSkin(skinIndex);
//                }
//            }
//        }

//        public void EquipSkin(Device device, int skinIndex)
//        {
//            device.ChangeSkin(skinIndex);

//            // Update the saved data
//            skinSelectionData.skinSelections[device.gameObject.name] = skinIndex;

//            // Save the updated data to Firebase
//            Manager.SaveManager.Save(skinSelectionData);
//        }
//    }
//}