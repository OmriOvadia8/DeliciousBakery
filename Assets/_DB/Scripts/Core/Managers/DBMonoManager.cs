using UnityEngine;

namespace DB_Core
{
    public class DBMonoManager
    {
        private DBMonoManagerObject monoObject;

        public DBMonoManager()
        {
            var temp = new GameObject("MonoManager");
            monoObject = temp.AddComponent<DBMonoManagerObject>();
            Object.DontDestroyOnLoad(monoObject);
        }
    }
}