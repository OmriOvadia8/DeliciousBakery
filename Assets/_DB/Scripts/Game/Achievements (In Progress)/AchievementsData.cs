using DB_Core;

namespace DB_Game
{
    public class AchievementData
    {
        public bool[] Cook10 { get; set; }
        public bool[] Cook100 { get; set; }
        public bool[] Cook500 { get; set; }
        public bool[] Cook1000 { get; set; }
        public bool[] Cook5000 { get; set; }
        public bool[] Hire10 { get; set; }
        public bool[] Hire100 { get; set; }
        public bool[] Hire500 { get; set; }
        public bool[] Hire1000 { get; set; }
        public bool[] Hire5000 { get; set; }
        public bool TotalCooked100 { get; set; }
        public bool TotalCooked1000 { get; set; }
        public bool TotalCooked5000 { get; set; }
        public bool TotalCooked10000 { get; set; }
        public bool TotalCooked20000 { get; set; }
        public bool TotalHired100 { get; set; }
        public bool TotalHired1000 { get; set; }
        public bool TotalHired5000 { get; set; }
        public bool TotalHired10000 { get; set; }
        public bool TotalHired20000 { get; set; }

        public AchievementData()
        {
            Cook10 = new bool[DBFoodManager.FOOD_COUNT];
            Cook100 = new bool[DBFoodManager.FOOD_COUNT];
            Cook500 = new bool[DBFoodManager.FOOD_COUNT];
            Cook1000 = new bool[DBFoodManager.FOOD_COUNT];
            Cook5000 = new bool[DBFoodManager.FOOD_COUNT];
            Hire10 = new bool[DBFoodManager.FOOD_COUNT];
            Hire100 = new bool[DBFoodManager.FOOD_COUNT];
            Hire500 = new bool[DBFoodManager.FOOD_COUNT];
            Hire1000 = new bool[DBFoodManager.FOOD_COUNT];
            Hire5000 = new bool[DBFoodManager.FOOD_COUNT];
        }
    }

    public class AchievementDataCollection : IDBSaveData
    {
        public AchievementData Achievements { get; set; }
    }

}
