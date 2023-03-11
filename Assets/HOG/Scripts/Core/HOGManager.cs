using System;
using Firebase.Crashlytics;
using Firebase.Extensions;

namespace Core
{
    public class HOGManager : IHOGBaseManager
    {

        public static HOGManager Instance;

        public HOGEventsManager EventsManager;
        public HOGFactoryManager FactoryManager;
        public HOGPoolManager PoolManager;
        public HOGSaveManager SaveManager;
        public HOGConfigManager ConfigManager;
        public HOGCrashManager CrashManager;
        public HOGAnalyticsManager AnalyticsManager;

        public Action onInitAction;

        public HOGManager()
        {
            if (Instance != null)
            {
                return;
            }

            Instance = this;
        }

        public void LoadManager(Action onComplete)
        {
            onInitAction = onComplete;
            InitFirebase(delegate
            {
                InitManagers();
            });
        }

        public void InitFirebase(Action onComplete)
        {
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    var app = Firebase.FirebaseApp.DefaultInstance;
                    HOGDebug.Log($"Firebase was initialized");
                    onComplete.Invoke();
                }
                else
                {
                    HOGDebug.LogException($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                }
            });
        }

        private void InitManagers()
        {
            HOGDebug.Log($"InitManagers");

            CrashManager = new HOGCrashManager();
            HOGDebug.Log($"After CrashManager");

            EventsManager = new HOGEventsManager();
            HOGDebug.Log($"After HOGEventsManager");

            AnalyticsManager = new HOGAnalyticsManager();

            FactoryManager = new HOGFactoryManager();
            HOGDebug.Log($"After HOGFactoryManager");

            PoolManager = new HOGPoolManager();
            HOGDebug.Log($"After HOGPoolManager");

            SaveManager = new HOGSaveManager();
            HOGDebug.Log($"After HOGSaveManager");

            HOGDebug.Log($"Before Config Manager");

            ConfigManager = new HOGConfigManager(delegate
            {
                onInitAction.Invoke();
            });

        }
    }
}