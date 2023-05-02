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
            for (int i = 0; i < DBFoodManager.FOOD_COUNT; i++)
            {
               // achievementsUI.achievementMakeFoodDescription[i].text = MakeFoodAchievementText();
            }
            
        }

        private void MakeFoodAchievementText(int amount, FoodTypes foodName)
        {
            string makeText = $"Make {amount} {foodName}";
        }

        private void HireBakerAchievementText()
        {

        }

        private void MakeTotalFoodAchievementText()
        {

        }

        private void HireTotalbBakersAchievementText()
        {

        }
    }








    [Serializable]
    public class AchievementsUIComponents
    {
        public TMP_Text[] achievementProgress;
        public TMP_Text[] achievementGoal;
        public TMP_Text[] achievementTitle;
        public TMP_Text[] achievementMakeFoodDescription;
        public TMP_Text[] achievementHireBakerDescription;
        public TMP_Text achievementTotalMakeFoodDescription;
        public TMP_Text achievementTotalHireBakerDescription;
        public Image[][] achievementStarsProgress;
        public GameObject[] achievementProgressBar;
        public Button[] achievementClaimButton;
    }

}