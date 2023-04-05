using System.Collections.Generic;
using Firebase.Analytics;
using UnityEngine.Device;

namespace DB_Core
{
    public class DBAnalyticsManager
    {
        public DBAnalyticsManager()
        {
            SetUserID();
        }

        public void ReportEvent(DBEventType eventType)
        {
            ReportEvent(eventType, new Dictionary<DBDataKeys, object>());
        }

        public void ReportEvent(DBEventType eventType, Dictionary<DBDataKeys, object> data)
        {
            var paramsData = new List<Parameter>();

            foreach (var keyVal in data)
            {
                if (keyVal.Value == null)
                {
                    continue;
                }

                var objType = keyVal.Value.GetType();

                var keyName = keyVal.Key.ToString();
                if (objType == typeof(string))
                {
                    paramsData.Add(new Parameter(keyName, (string)keyVal.Value));
                }
                else if (objType == typeof(float))
                {
                    paramsData.Add(new Parameter(keyName, (float)keyVal.Value));
                }
                else if (objType == typeof(int))
                {
                    paramsData.Add(new Parameter(keyName, (int)keyVal.Value));
                }
                else if (objType == typeof(bool))
                {
                    paramsData.Add(new Parameter(keyName, (bool)keyVal.Value ? 1 : 0));
                }
            }

            FirebaseAnalytics.LogEvent(eventType.ToString(), paramsData.ToArray());
        }

        public void SetUserProperties(Dictionary<string, string> data)
        {
            foreach (var keyVal in data)
            {
                SetUserProperty(keyVal.Key, keyVal.Value);
            }
        }

        public void SetUserProperty(string key, string val)
        {
            FirebaseAnalytics.SetUserProperty(key, val);
        }

        public void SetUserID()
        {
            FirebaseAnalytics.SetUserId(SystemInfo.deviceUniqueIdentifier);
        }
    }

    public enum DBEventType
    {
        app_loaded,
        upgrade_item,
        try_upgrade_out_of,
        popup_open,
        popup_impression,
        popup_close,
        ad_show_start,
        ad_show_click,
        ad_show_complete,
        purchase_complete
    }

    public enum DBDataKeys
    {
        type_id,
        upgrade_level,
        popup_type,
        product_id,
        product_receipt
    }

}
