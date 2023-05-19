using DB_Core;
using UnityEngine;

namespace DB_Game
{
    public class DBPingManager : DBLogicMonoBehaviour
    {
        [SerializeField] private GameObject achievementPing;
        [SerializeField] private GameObject bakerPing;

        private void OnEnable() => RegisterEvents();

        private void OnDisable() => UnregisterEvents();

        private void ShowAchievementPing(object value)
        {
            bool activeValue = (bool)value;
            achievementPing.SetActive(activeValue);
        }

        private void ShowBakerPing(object value)
        {
            bool activeValue = (bool)value;
            bakerPing.SetActive(activeValue);
        }

        private void RegisterEvents()
        {
            AddListener(DBEventNames.AchievementPing, ShowAchievementPing);
            AddListener(DBEventNames.BakerPing, ShowBakerPing);
        }

        private void UnregisterEvents()
        {
            RemoveListener(DBEventNames.AchievementPing, ShowAchievementPing);
            RemoveListener(DBEventNames.BakerPing, ShowBakerPing);
        }
    }
}