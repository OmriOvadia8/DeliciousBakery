using System;
using System.Diagnostics;
using Firebase.Crashlytics;

namespace Core
{
    public class HOGCrashManager
    {
        public HOGCrashManager()
        {
            HOGDebug.Log($"HOGCrashManager");

            Crashlytics.ReportUncaughtExceptionsAsFatal = true;
        }

        public void LogExceptionHandling(string message)
        {
            Crashlytics.LogException(new Exception(message));
            HOGDebug.LogException(message);
        }

        public void LogBreadcrumb(string message)
        {
            Crashlytics.Log(message);
            HOGDebug.Log(message);
        }
    }
}