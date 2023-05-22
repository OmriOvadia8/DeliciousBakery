using System.Collections;
using UnityEngine;

namespace DB_Game
{
    public class DBRandomCurrencyManager : FoodDataAccess
    {
        [SerializeField] RectTransform spawnArea;
        [SerializeField] GameObject coinPrefab;
        [SerializeField] GameObject starPrefab;

        private void Start()
        {
            StartCoroutine(SpawnCoins());
            StartCoroutine(SpawnStars());
        }

        private IEnumerator SpawnCoins()
        {
            while (true)
            {
                if (!IsFirstFoodLocked())
                {
                    yield return new WaitForSeconds(35);

                    Vector2 randomPositionWithinObject = GetRandomPosition();

                    RectTransform coinTransform = coinPrefab.GetComponent<RectTransform>();
                    coinTransform.SetParent(spawnArea, false);
                    coinTransform.anchoredPosition = randomPositionWithinObject;

                    coinPrefab.SetActive(true);
                }
                else
                {
                    yield return null; 
                }
            }
        }

        private IEnumerator SpawnStars()
        {
            while (true)
            {
                if (!IsFirstFoodLocked())
                {
                    yield return new WaitForSeconds(65);

                    Vector2 randomPositionWithinObject = GetRandomPosition();

                    RectTransform starTransform = starPrefab.GetComponent<RectTransform>();
                    starTransform.SetParent(spawnArea, false);
                    starTransform.anchoredPosition = randomPositionWithinObject;

                    starPrefab.SetActive(true);
                }
                else
                {
                    yield return null; 
                }
            }
        }

        private Vector2 GetRandomPosition()
        {
            return new Vector2(
                Random.Range(spawnArea.rect.xMin, spawnArea.rect.xMax),
                Random.Range(spawnArea.rect.yMin, spawnArea.rect.yMax)
                );
        }

        private bool IsFirstFoodLocked()
        {
            bool isFirstFoodLocked = GetFoodData(0).IsFoodLocked;
            return isFirstFoodLocked;
        }
    }
}
