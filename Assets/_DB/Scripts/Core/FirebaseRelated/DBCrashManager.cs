using System;
using System.Diagnostics;
using Firebase.Crashlytics;

namespace DB_Core
{
    public class DBCrashManager
    {
        public DBCrashManager()
        {
            DBDebug.Log($"DBCrashManager");

            Crashlytics.ReportUncaughtExceptionsAsFatal = true;
        }

        public void LogExceptionHandling(string message)
        {
            Crashlytics.LogException(new Exception(message));
            DBDebug.LogException(message);
        }

        public void LogBreadcrumb(string message)
        {
            Crashlytics.Log(message);
            DBDebug.Log(message);
        }
    }
}