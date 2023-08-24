using UnityEngine;

public class MainCameraSizeChange : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] BoardSystem boardSystem;

    private void Start()
    {
        Vector3 newPosition = new Vector3(
            mainCamera.transform.position.x + (int)(boardSystem.Width / 2f),
            mainCamera.transform.position.y + (int)(boardSystem.Height / 2f),
            mainCamera.transform.position.z
        );

        mainCamera.transform.position = newPosition;
        mainCamera.orthographicSize = boardSystem.Width + 1;
    }

    private void LogMsg(object msg)
    {
        string mesg = (string)msg;
        Debug.Log(mesg);    
    }
}
