using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public class HOGGameLoader : HOGMonoBehaviour
    {
        [SerializeField] private HOGGameLoaderBase gameLogicLoader;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            Invoke(nameof(DelayStart), 0.1f);
        }

        private void DelayStart()
        {
            var manager = new HOGManager();

            manager.LoadManager(() =>
            {
                gameLogicLoader.StartLoad(() =>
                {
                    SceneManager.LoadScene(1);
                    Manager.AnalyticsManager.ReportEvent(HOGEventType.app_loaded);
                    Destroy(gameObject);
                });
            });
        }
    }
}