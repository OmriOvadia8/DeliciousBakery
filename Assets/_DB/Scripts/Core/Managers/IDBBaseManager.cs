using System;

namespace DB_Core
{
    public interface IDBBaseManager
    {
        public void LoadManager(Action onComplete);
    }
}