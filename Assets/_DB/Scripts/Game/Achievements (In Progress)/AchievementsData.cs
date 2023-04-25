using DB_Core;
using System.Collections.Generic;

namespace DB_Game
{
    public class AchievementsData
    {
        public bool Cooked10Food { get; set; }
        public bool Cooked30Food { get; set; }
        public bool Cooked50Food { get; set; }
        public bool Cooked100Food { get; set; }
        public bool Cooked250Food { get; set; }
        public bool Cooked500Food { get; set; }
        public bool Cooked1000Food { get; set; }

        public bool Hired10Bakers { get; set; }
        public bool Hired30Bakers { get; set; }
        public bool Hired50Bakers { get; set; }
        public bool Hired100Bakers { get; set; }
        public bool Hired250Bakers { get; set; }
        public bool Hired500Bakers { get; set; }
        public bool Hired1000Bakers { get; set; }

        public bool Cooked100TotalFood { get; set; }
        public bool Cooked500TotalFood { get; set; }
        public bool Cooked1000TotalFood { get; set; }
        public bool Cooked2500TotalFood { get; set; }
        public bool Cooked5000TotalFood { get; set; }
        public bool Cooked10000TotalFood { get; set; }

        public bool Hired100TotalBakers { get; set; }
        public bool Hired500TotalBakers { get; set; }
        public bool Hired1000TotalBakers { get; set; }
        public bool Hired2500TotalBakers { get; set; }
        public bool Hired5000TotalBakers { get; set; }
        public bool Hired10000TotalBakers { get; set; }
        public AchievementsData(bool cooked10Food, bool cooked30Food, bool cooked50Food,
                                bool cooked100Food, bool cooked250Food, bool cooked500Food,
                                bool cooked1000Food,bool hired10Bakers, bool hired30Bakers,
                                bool hired50Bakers, bool hired100Bakers, bool hired250Bakers,
                                bool hired500Bakers, bool hired1000Bakers,bool cooked100TotalFood,
                                bool cooked500TotalFood, bool cooked1000TotalFood, bool cooked2500TotalFood,
                                bool cooked5000TotalFood, bool cooked10000TotalFood,bool hired100TotalBakers,
                                bool hired500TotalBakers, bool hired1000TotalBakers, bool hired2500TotalBakers, 
                                bool hired5000TotalBakers, bool hired10000TotalBakers)
        {
            Cooked10Food = cooked10Food;
            Cooked30Food = cooked30Food;
            Cooked50Food = cooked50Food;
            Cooked100Food = cooked100Food;
            Cooked250Food = cooked250Food;
            Cooked500Food = cooked500Food;
            Cooked1000Food = cooked1000Food;

            Hired10Bakers = hired10Bakers;
            Hired30Bakers = hired30Bakers;
            Hired50Bakers = hired50Bakers;
            Hired100Bakers = hired100Bakers;
            Hired250Bakers = hired250Bakers;
            Hired500Bakers = hired500Bakers;
            Hired1000Bakers = hired1000Bakers;

            Cooked100TotalFood = cooked100TotalFood;
            Cooked500TotalFood = cooked500TotalFood;
            Cooked1000TotalFood = cooked1000TotalFood;
            Cooked2500TotalFood = cooked2500TotalFood;
            Cooked5000TotalFood = cooked5000TotalFood;
            Cooked10000TotalFood = cooked10000TotalFood;

            Hired100TotalBakers = hired100TotalBakers;
            Hired500TotalBakers = hired500TotalBakers;
            Hired1000TotalBakers = hired1000TotalBakers;
            Hired2500TotalBakers = hired2500TotalBakers;
            Hired5000TotalBakers = hired5000TotalBakers;
            Hired10000TotalBakers = hired10000TotalBakers;
        }
    }

    public class AchievementsDataCollection : IDBSaveData
    {
        public List<AchievementsData> AchievementsList { get; set; }

        public AchievementsDataCollection()
        {
            AchievementsList = new List<AchievementsData>();
        }
    }

}
