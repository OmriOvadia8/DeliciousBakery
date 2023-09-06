using System;
using Unity.Services.Core;
using Unity.Services.Core.Environments;

namespace DB_Core
{
    public class InitializeUnityServices : DBMonoBehaviour
    {
        public string environment = "production";

        async void Awake()
        {
            try
            {
                var options = new InitializationOptions()
                    .SetEnvironmentName(environment);

                await UnityServices.InitializeAsync(options);
            }
            catch (Exception exception)
            {
                DBDebug.LogException(exception);
                Manager.AnalyticsManager.ReportEvent(DBEventType.unity_services_failed);
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}