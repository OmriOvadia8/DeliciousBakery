using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace DB_Core
{
    public class DBDebug
    {
        [Conditional("LOGS_ENABLE")]
        public static void Log(object message)
        {
            Debug.Log(message);
        }

        [Conditional("LOGS_ENABLE")]
        public static void LogException(object message)
        {
            Debug.LogException(new Exception(message.ToString()));
        }
    }
}