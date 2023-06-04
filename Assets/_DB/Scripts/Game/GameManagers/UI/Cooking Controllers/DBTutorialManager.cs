using UnityEngine;

namespace DB_Game
{
    public class DBTutorialManager : FoodDataAccess
    {
        [SerializeField] GameObject cookHandpoint;
        [SerializeField] GameObject hireHandpoint;

        private void Start() => HandpointAppearance();

        private void HandpointAppearance()
        {
            cookHandpoint.SetActive(false);

            var firstFoodCooked = GetFoodData(0).FoodCookedCount;
            var firstFoodLocked = GetFoodData(0).IsFoodLocked;

            if (firstFoodCooked == 0 && !firstFoodLocked)
            {
                cookHandpoint.SetActive(true);
                hireHandpoint.SetActive(true);
            }
        }

        public void ToggleHandpoints() => HandpointAppearance();

        public void TurnOffCookHandpoint() => cookHandpoint.SetActive(false);
        public void TurnOffHireHandpoint() => hireHandpoint.SetActive(false);
    }
}