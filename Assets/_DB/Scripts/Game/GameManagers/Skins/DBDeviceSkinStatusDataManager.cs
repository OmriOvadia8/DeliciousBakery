using DB_Core;

namespace DB_Game
{
    public class DBDeviceSkinStatusDataManager : DBLogicMonoBehaviour
    {
        public DeviceSkinUnlockedData SkinUnlockData;
        private const string SKINS_UNLOCK_CONFIG_PATH = "skins_data";

        private void OnEnable() => LoadSkinStatus();

        private void LoadSkinStatus()
        {
            Manager.SaveManager.Load<DeviceSkinUnlockedData>(data =>
            {
                if (data != null)
                {
                    SkinUnlockData = data;
                    DBDebug.Log("Skin data");
                }
                else
                {
                    Manager.ConfigManager.GetConfigAsync<DeviceSkinUnlockedData>(SKINS_UNLOCK_CONFIG_PATH, OnSkinConfigLoaded);
                    DBDebug.Log("Skin data ELSE");
                }
            });
        }

        private void OnSkinConfigLoaded(DeviceSkinUnlockedData config)
        {
            SkinUnlockData = config;
            DBDebug.Log("Skin config on loadded");
        }

        public void SaveSkinsUnlockData()
        {
            Manager.SaveManager.Save(SkinUnlockData);
        }
    }
}