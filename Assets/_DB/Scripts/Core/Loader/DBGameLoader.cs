using UnityEngine;
using UnityEngine.SceneManagement;

namespace DB_Core
{
    public class DBGameLoader : DBMonoBehaviour
    {
        [SerializeField] private DBGameLoaderBase gameLogicLoader;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            WaitForFrame(DelayStart);
        }

        private void DelayStart()
        {
            var manager = new DBManager();

            manager.LoadManager(() =>
            {
                gameLogicLoader.StartLoad(() =>
                {
                    SceneManager.LoadScene(1);
                    Manager.AnalyticsManager.ReportEvent(DBEventType.app_loaded);
                    ShowMessage();
                });
            });
        }

        private void ShowMessage()
        {
            int offlineTime = Manager.TimerManager.GetLastOfflineTimeSeconds();

            WaitForFrame(() =>
            {
                if(offlineTime == 0)
                {
                    Manager.PopupManager.AddPopupToQueue(DBPopupData.NoProfitWelcome);
                }

                else
                {
                    Manager.PopupManager.AddPopupToQueue(DBPopupData.WelcomeBackMessage);
                }

                Destroy(gameObject);
            });
        }
    }
}