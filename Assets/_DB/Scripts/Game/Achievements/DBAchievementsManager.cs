//using DB_Core;
//using UnityEngine;
//using TMPro;
//using System.Collections.Generic;
//using System.Linq;

//namespace DB_Game
//{
//    public class DBAchievementsManager : FoodDataAccess
//    {
//        [SerializeField] TMP_Text[][] AchievementsTexts;
//        [SerializeField] TMP_Text[] tenFoodCookedCountText;
//        [SerializeField] TMP_Text[] thirtyFoodCookedCountText;
//        [SerializeField] TMP_Text[] fiftyFoodCookedCountText;
//        [SerializeField] TMP_Text[] hundredFoodCookedCountText;
//        [SerializeField] TMP_Text[] twofiftyFoodCookedCountText;
//        [SerializeField] TMP_Text[] fivehFoodCookedCountText;
//        [SerializeField] TMP_Text[] thousandCookedCountText;
//        [SerializeField] TMP_Text[] bakersCountText;
//        [SerializeField] TMP_Text totalFoodCookedText;
//        [SerializeField] TMP_Text totalBakersText;

//        private Achievements[] achievements;
//      //  private Achievements cook10times = new Achievements(DBFoodManager.GetFoodData(0), "food", 10, 10, AchievementsTexts[0][1]);

//        private int totalFoodCooked = 0;
//        private int totalBakers = 0;

//        private void Awake()
//        {
//            //for (int i = 0; i < DBFoodManager.FOOD_COUNT; i++)
//            //{

//            //}

//            //if(cook10times.Food == DBFoodManager.GetFoodData(0))
//            //{
//                //if(cook10times.Amount <= DBFoodManager.GetFoodData(0).FoodCookedCount)
//             //   {
//              //      cook10times.Completed = true;
//           //     }
//         //   }
//        }

//        private void OnEnable()
//        {
//            AddListener(DBEventNames.OnCompleteCookFood, FoodCookedUpdateUI);
//            AddListener(DBEventNames.OnHired, BakerHiredUpdateUI);
//        }

//        private void Start()
//        {

//            AchievementsUISetup();
//        }

//        private void OnDisable()
//        {
//            RemoveListener(DBEventNames.OnCompleteCookFood, FoodCookedUpdateUI);
//            RemoveListener(DBEventNames.OnHired, BakerHiredUpdateUI);
//        }

//        private void FoodCookedUpdateUI(object foodIndex)
//        {
//            int index = (int)foodIndex;
//            var foodData = foodDataRepository.GetFoodData(index);

//        }

//        private void BakerHiredUpdateUI(object foodIndex)
//        {
//            int index = (int)foodIndex;
//            bakersCountText[index].text = foodDataRepository.GetFoodData(index).BakersCount.ToString();
//        }

     


//        private void AchievementsUISetup()
//        {
//            for (int i = 0; i < DBFoodManager.FOOD_COUNT; i++)
//            {
//                var foodData = foodDataRepository.GetFoodData(i);

//                //foodCookedCountText[i].text = foodData.FoodCookedCount.ToString();
//                bakersCountText[i].text = foodData.BakersCount.ToString();

//                totalFoodCooked += foodData.FoodCookedCount;
//                totalBakers += foodData.BakersCount;

//            }

//            totalFoodCookedText.text = totalFoodCooked.ToString();
//            totalBakersText.text = totalBakers.ToString();
//        }

//        private void RewardPremCurrency(int reward)
//        {
//            GameLogic.ScoreManager.ChangeScoreByTagByAmount(ScoreTags.PremiumCurrency, reward);
//        }
//    }
//}
