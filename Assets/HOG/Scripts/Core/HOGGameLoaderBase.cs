using System;

namespace Core
{
    public class HOGGameLoaderBase : HOGMonoBehaviour
    {
        public virtual void StartLoad(Action onComplete)
        {
            onComplete.Invoke();
        }
    }
}