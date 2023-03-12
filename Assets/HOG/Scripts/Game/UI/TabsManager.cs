using UnityEngine.UI;
using UnityEngine;
using Core;

namespace Game
{
    public class TabsManager : MonoBehaviour
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
                HOGDebug.LogException($"Invalid tab index: {tabIndex}");
                return;
            }

            scrollers[currentTab].SetActive(false);
            tabButtons[currentTab].interactable = true;

            currentTab = tabIndex;

            scrollers[currentTab].SetActive(true);
            tabButtons[currentTab].interactable = false;
        }
    }
}