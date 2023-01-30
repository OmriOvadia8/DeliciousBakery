
using UnityEngine;

public class HOGTester : HOGMonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(Manager.GetNumber());
        }
    }
}