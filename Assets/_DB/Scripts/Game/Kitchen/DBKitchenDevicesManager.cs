using UnityEngine;
using DB_Core;

namespace DB_Game
{
    public class DBKitchenDevicesManager : DBLogicMonoBehaviour
    {
        [SerializeField] GameObject[] deviceObject;
        [SerializeField] GameObject[] poofObjects;
        [SerializeField] Animator[] poofAnims;

        private void OnEnable()
        {
            AddListener(DBEventNames.DeviceAppearAnimation, DeviceAppear);
        }

        private void OnDisable()
        {
            RemoveListener(DBEventNames.DeviceAppearAnimation, DeviceAppear);
        }

        private void Start()
        {
            for (int i = 0; i < poofObjects.Length; i++)
            {
                bool isFoodLocked = DBFoodManager.GetFoodData(i).IsFoodLocked;
                deviceObject[i].SetActive(!isFoodLocked);
                poofObjects[i].SetActive(!isFoodLocked);

                if (deviceObject[i].activeSelf)
                {
                    poofObjects[i].SetActive(false);
                }
            }
        }

        private void DeviceAppear(object obj)
        {
            int index = (int)obj;
            if(index == 8 || index == 9) // only have 8 devices therefore ignoring index 8 and 9
            {
                return;
            }
            deviceObject[index].SetActive(true);
            poofObjects[index].SetActive(true);
            poofAnims[index].SetBool("deviceAppear", true);
        }

    }
}