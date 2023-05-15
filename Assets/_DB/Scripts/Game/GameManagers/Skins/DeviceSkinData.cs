using DB_Core;

namespace DB_Game
{
    public class DeviceSkinData
    {
        public bool Color1 { get; set; }
        public bool Color2 { get; set; }
        public int Equipped { get; set; }

        public DeviceSkinData()
        {
        }

        public DeviceSkinData(bool color1, bool color2, int equipped)
        {
            Color1 = color1;
            Color2 = color2;
            Equipped = equipped;
        }
    }

    public class DeviceSkinUnlockedData : IDBSaveData
    {
        public DeviceSkinData[] Skins { get; set; }
    }
}