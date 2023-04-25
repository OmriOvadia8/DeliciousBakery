using UnityEngine;
using DB_Core;

namespace DB_Game
{
    public class DBKitchenDevicesManager : FoodDataAccess
    {
        [SerializeField] private GameObject[] deviceObject;
        [SerializeField] private GameObject[] poofObjects;
        [SerializeField] private Animator[] poofAnims;

        private const int MAX_DEVICES = 8;

        private void OnEnable() => AddListener(DBEventNames.DeviceAppearAnimation, KitchenDeviceAppear);

        private void OnDisable() => RemoveListener(DBEventNames.DeviceAppearAnimation, KitchenDeviceAppear);

        private void Start() => InitializeDevicesAndPoofs();

        private void InitializeDevicesAndPoofs()
        {
            for (int i = 0; i < poofObjects.Length; i++)
            {
                bool isFoodLocked = GetFoodData(i).IsFoodLocked;
                SetDeviceAndPoofActiveState(i, !isFoodLocked);
            }
        }

        private void KitchenDeviceAppear(object obj)
        {
            int index = (int)obj;

            if (index >= MAX_DEVICES) 
            {
                return;
            }

            ActivateDeviceWithAnimation(index);
        }

        private void SetDeviceAndPoofActiveState(int index, bool isDeviceActive)
        {
            deviceObject[index].SetActive(isDeviceActive);
            poofObjects[index].SetActive(!isDeviceActive);
        }

        private void ActivateDeviceWithAnimation(int index)
        {
            deviceObject[index].SetActive(true);
            poofObjects[index].SetActive(true);
            poofAnims[index].SetBool("deviceAppear", true);
        }
    }
}
