using System;

namespace DB_Core
{
    public class DBGameLoaderBase : DBMonoBehaviour
    {
        public virtual void StartLoad(Action onComplete)
        {
            onComplete.Invoke();
        }
    }
}