using UnityEngine.SceneManagement;

namespace Core
{
    public class HOGGameLoader : HOGMonoBehaviour
    {
        private void Start()
        {
            Invoke("DelayStart", 0.1f);
        }

        private void DelayStart()
        {
            new HOGManager();
            SceneManager.LoadScene(1);
            Destroy(this.gameObject);
        }
    }
}