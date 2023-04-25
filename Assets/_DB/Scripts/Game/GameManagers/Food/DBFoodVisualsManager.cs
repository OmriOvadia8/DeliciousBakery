using DB_Core;
using UnityEngine;

namespace DB_Game
{
    public class DBFoodVisualsManager : DBLogicMonoBehaviour
    {
        [SerializeField] GameObject[] LockedFoodBars;
        [SerializeField] GameObject[] LockedBakersBars;
        [SerializeField] ParticleSystem[] cookParticles;
        [SerializeField] ParticleSystem[] upgradeParticles;
        [SerializeField] ParticleSystem[] hireParticles;
        [SerializeField] ParticleSystem[] learnParticles;

        private void OnEnable() => RegisterEvents();

        private void OnDisable() => UnregisterEvents();

        private void PlayCookParticles(object foodIndex) => cookParticles[(int)foodIndex].Play();

        private void PlayHireParticle(object foodIndex) => hireParticles[(int)foodIndex].Play();

        private void PlayLearnParticles(object foodIndex) => learnParticles[(int)foodIndex].Play();

        private void PlayUpgradeParticles(object foodIndex) => upgradeParticles[(int)foodIndex].Play();

        private void RevealFoodBars(object foodIndex) => SetLockedFoodBarsActiveState(foodIndex, false);

        private void HideFoodBars(object foodIndex) => SetLockedFoodBarsActiveState(foodIndex, true);

        private void SetLockedFoodBarsActiveState(object foodIndex, bool state)
        {
            int index = (int)foodIndex;
            LockedFoodBars[index].SetActive(state);
            LockedBakersBars[index].SetActive(state);
        }

        private void RegisterEvents()
        {
            AddListener(DBEventNames.CookParticles, PlayCookParticles);
            AddListener(DBEventNames.BakerParticles, PlayHireParticle);
            AddListener(DBEventNames.LearnParticles, PlayLearnParticles);
            AddListener(DBEventNames.UpgradeParticles, PlayUpgradeParticles);
            AddListener(DBEventNames.FoodBarReveal, RevealFoodBars);
            AddListener(DBEventNames.FoodBarLocked, HideFoodBars);
        }

        private void UnregisterEvents()
        {
            RemoveListener(DBEventNames.BakerParticles, PlayHireParticle);
            RemoveListener(DBEventNames.LearnParticles, PlayLearnParticles);
            RemoveListener(DBEventNames.UpgradeParticles, PlayUpgradeParticles);
            RemoveListener(DBEventNames.FoodBarReveal, RevealFoodBars);
            RemoveListener(DBEventNames.FoodBarLocked, HideFoodBars);
            RemoveListener(DBEventNames.CookParticles, PlayCookParticles);
        }
    }
}