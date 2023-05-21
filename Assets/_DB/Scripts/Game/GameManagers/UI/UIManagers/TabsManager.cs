using UnityEngine.UI;
using UnityEngine;
using DB_Core;

namespace DB_Game
{
    public class TabsManager : DBLogicMonoBehaviour
    {
        [SerializeField] GameObject[] scrollers;
        [SerializeField] Button[] tabButtons;

        private int currentTab = 0;

        private void Start()
        {
            ShowTab(currentTab);
        }

        public void ShowTab(int tabIndex)
        {
            if (tabIndex < 0 || tabIndex >= scrollers.Length)
            {
                DBDebug.LogException($"Invalid tab index: {tabIndex}");
                return;
            }

            scrollers[currentTab].SetActive(false);
            tabButtons[currentTab].interactable = true;

            currentTab = tabIndex;

            scrollers[currentTab].SetActive(true);
            tabButtons[currentTab].interactable = false;

            InvokeEvent(DBEventNames.PlaySound, SoundEffectType.TabSound);

            TurnOffPing(tabIndex);
        }

        private void TurnOffPing(int tabIndex)
        {
            switch (tabIndex)
            {
                case 1:
                    InvokeEvent(DBEventNames.BakerPing, false);
                    break;

                case 2:
                    InvokeEvent(DBEventNames.AchievementPing, false);
                    break;
            }
        }
    }
}