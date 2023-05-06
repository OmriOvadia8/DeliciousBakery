using System;
using DB_Core;

namespace DB_Game
{
    [Serializable]
    public class AchievementClaimData : IDBSaveData
    {
        public bool[,] IsMakeFoodRewardClaimed;
        public bool[,] IsHireBakerRewardClaimed;

        public AchievementClaimData(int foodItemCount, int rewardsPerFoodItem)
        {
            IsMakeFoodRewardClaimed = new bool[foodItemCount, rewardsPerFoodItem];
            IsHireBakerRewardClaimed = new bool[foodItemCount, rewardsPerFoodItem];
        }
    }
}
