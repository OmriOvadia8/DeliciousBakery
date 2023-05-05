using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

namespace DB_Game
{
    public class AchievementsDOTweenController : FoodDataAccess
    {

    }

    [Serializable]
    public class AchievementsDOTweenMakeFoodComponents
    {
        public TMP_Text[] AchievementMakeFoodProgress;
        public TMP_Text[] AchievementMakeFoodGoal;
        public Button[] AchievementMakeFoodClaimButton;
    }

    [Serializable]
    public class AchievementsDOTweenHireBakerComponents
    {
        public TMP_Text[] AchievementHireBakerProgress;
        public TMP_Text[] AchievementHireBakerGoal;
        public Button[] AchievementHireBakerClaimButton;
    }
}