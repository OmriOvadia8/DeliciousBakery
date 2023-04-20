using DB_Core;

namespace DB_Game
{
    public class DBCookingManager : DBLogicMonoBehaviour
    {
        private void OnEnable()
        {
           // AddListener(DBEventNames.StartActiveCooking, CookFood);
           // AddListener(DBEventNames.StartBakerCooking, AutoCookFood);
        }

        private void OnDisable()
        {
            //RemoveListener(DBEventNames.StartActiveCooking, CookFood);
          //  RemoveListener(DBEventNames.StartBakerCooking, AutoCookFood);
        }

        //public void CookFood(int index) // Active cooking by clicking
        //{
        //    //int index = (int)foodIndex;
            
        //    var foodData = DBFoodManager.GetFoodData(index);

        //    if (foodData.IsOnCooldown)
        //    {
        //        return;
        //    }

        //    foodData.IsOnCooldown = true;

        //    DBManager.Instance.SaveManager.Save(DBFoodManager.Foods);
        //    InvokeEvent(DBEventNames.CookFoodButtonCheck, null);
        //    InvokeEvent(DBEventNames.OnCookFood, index); // starts the loading bar and timer of cooking
        //}

        //public void AutoCookFood(int index) // Cooking automatically courotine loop after baker unlocked
        //{

        //    var foodData = DBFoodManager.GetFoodData(index);

        //    if (foodData.IsAutoOnCooldown)
        //    {
        //        return;
        //    }
        //    foodData.IsAutoOnCooldown = true;

        //    InvokeEvent(DBEventNames.OnAutoCookFood, index); // starts the loading bar and timer of cooking
        //}
    }
}