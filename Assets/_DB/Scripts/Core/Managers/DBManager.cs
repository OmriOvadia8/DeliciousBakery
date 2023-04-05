using System;
using Firebase.Extensions;

namespace DB_Core
{
    public class DBManager : IDBBaseManager
    {
        public static DBManager Instance;

        public DBEventsManager EventsManager;
        public DBFactoryManager FactoryManager;
        public DBPoolManager PoolManager;
        public DBSaveManager SaveManager;
        public DBConfigManager ConfigManager;
        public DBCrashManager CrashManager;
        public DBAnalyticsManager AnalyticsManager;
        public DBTimeManager TimerManager;
        public DBMonoManager MonoManager;
        public DBInAppPurchase PurchaseManager;
        public DBAdsManager AdsManager;

        public Action onInitAction;

        public DBManager()
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
                    DBDebug.Log($"Firebase was initialized");
                    onComplete.Invoke();
                }
                else
                {
                    DBDebug.LogException($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                }
            });
        }

        private void InitManagers()
        {
            MonoManager = new DBMonoManager();
            DBDebug.Log($"InitManagers");

            CrashManager = new DBCrashManager();
            DBDebug.Log($"After CrashManager");

            EventsManager = new DBEventsManager();
            DBDebug.Log($"After DBEventsManager");

            AnalyticsManager = new DBAnalyticsManager();

            FactoryManager = new DBFactoryManager();
            DBDebug.Log($"After DBFactoryManager");

            PoolManager = new DBPoolManager();
            DBDebug.Log($"After DBPoolManager");

            SaveManager = new DBSaveManager();
            DBDebug.Log($"After DBSaveManager");

            DBDebug.Log($"Before Config Manager");

            TimerManager = new DBTimeManager();

            PurchaseManager = new DBInAppPurchase();

            AdsManager = new DBAdsManager();

            ConfigManager = new DBConfigManager(delegate
            {
                onInitAction.Invoke();
            });

        }

    }
}