using UnityEngine;

namespace Core
{
    public class HOGMonoManager
    {
        private HOGMonoManagerObject monoObject;

        public HOGMonoManager()
        {
            var temp = new GameObject("MonoManager");
            monoObject = temp.AddComponent<HOGMonoManagerObject>();
            Object.DontDestroyOnLoad(monoObject);
        }
    }
}