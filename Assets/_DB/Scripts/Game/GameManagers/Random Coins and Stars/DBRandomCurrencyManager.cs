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
                    float randomDelay = Random.Range(15f, 25f);
                    yield return new WaitForSeconds(randomDelay);

                    Vector2 randomPositionWithinObject = GetRandomPosition();

                    RectTransform coinTransform = (RectTransform)coinPrefab.transform;

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
                    float randomDelay = Random.Range(35f, 50f);
                    yield return new WaitForSeconds(randomDelay);

                    Vector2 randomPositionWithinObject = GetRandomPosition();

                    RectTransform starTransform = (RectTransform)starPrefab.transform;
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
