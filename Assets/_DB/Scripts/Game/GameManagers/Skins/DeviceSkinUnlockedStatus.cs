using DB_Core;

namespace DB_Game
{
    public class DeviceSkinUnlockedStatus
    {
        public bool Color1 { get; set; }
        public bool Color2 { get; set; }

        public DeviceSkinUnlockedStatus()
        {
        }

        public DeviceSkinUnlockedStatus(bool color1, bool color2)
        {
            Color1 = color1;
            Color2 = color2;
        }
    }

    public class DeviceSkinUnlockedData : IDBSaveData
    {
        public DeviceSkinUnlockedStatus[] Skins { get; set; }
    }
}