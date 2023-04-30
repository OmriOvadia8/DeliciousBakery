using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DB_Game
{
    public class DBAchievementsText : FoodDataAccess
    {

        [Header("Achievements UI Components")]
        [SerializeField] AchievementsUIComponents achievementsUI;
        


        private void InitialAchievementsText()
        {

        }

        private void MakeText()
        {
            
        }
    }








    [Serializable]
    public class AchievementsUIComponents
    {
        public TMP_Text[] achievementProgress;
        public TMP_Text[] achievementGoal;
        public TMP_Text[] achievementTitle;
        public TMP_Text[] achievementDescription;
        public Image[][] achievementStarsProgress;
        public GameObject[] achievementProgressBar;
        public Button[] achievementClaimButton;
    }

}