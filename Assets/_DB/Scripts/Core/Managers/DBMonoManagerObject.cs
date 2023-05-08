namespace DB_Core
{
    public class DBMonoManagerObject : DBMonoBehaviour
    {
        private void OnApplicationPause(bool pauseStatus)
        {
            Manager.EventsManager.InvokeEvent(DBEventNames.OnPause, pauseStatus);
        }
    }
}