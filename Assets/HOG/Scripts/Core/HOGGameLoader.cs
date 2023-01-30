public class HOGGameLoader : HOGMonoBehaviour
{
    private void Start()
    {
        Invoke("DelayStart", 0.1f);
    }

    private void DelayStart()
    {
        new HOGManager();
        Destroy(this.gameObject);
    }
}